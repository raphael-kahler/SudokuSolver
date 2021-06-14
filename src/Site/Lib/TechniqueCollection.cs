using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SudokuSolver;
using SudokuSolver.Techniques;

namespace Site.Lib
{
    public interface ITechnique
    {
        string Name { get; }
        bool IsCollection { get; }
        IChangeDescription FoundChange { get; }
        event EventHandler<IChangeDescription> ChangeDescriptionUpdated;

        void ClearSolverChanges();
        Task<IChangeDescription> FindChangeFor(BoardState board);
        Task<IChangeDescription> FindNextChangeFor(BoardState board);
    }

    public class TechniqueCollection : ITechnique
    {
        public bool IsCollection => true;
        public string Name { get; }
        public IReadOnlyList<ITechnique> Techniques { get; }
        public IChangeDescription FoundChange { get; private set; }
        public event EventHandler<IChangeDescription> ChangeDescriptionUpdated;

        public TechniqueCollection(string name, IReadOnlyList<ITechnique> techniques)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            Techniques = techniques ?? throw new System.ArgumentNullException(nameof(techniques));
        }

        public async Task<IChangeDescription> FindChangeFor(BoardState board)
        {
            var changes = await Task.WhenAll<IChangeDescription>(Techniques.Select(async technique => await technique.FindChangeFor(board)));
            FoundChange = changes.FirstOrDefault(change => change.Change.HasEffect) ?? NoChangeDescription.Instance;
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
            return FoundChange;
        }

        public async Task<IChangeDescription> FindNextChangeFor(BoardState board)
        {
            if (FoundChange != null)
            {
                return FoundChange;
            }
            var nextTechnique = Techniques.FirstOrDefault(t => t.FoundChange == null);
            if (nextTechnique != null)
            {
                await nextTechnique.FindNextChangeFor(board);
            }
            if (Techniques.All(t => t.FoundChange != null))
            {
                FoundChange = Techniques.FirstOrDefault(t => t.FoundChange.Change.HasEffect)?.FoundChange ?? NoChangeDescription.Instance;
                ChangeDescriptionUpdated?.Invoke(this, FoundChange);
                return FoundChange;
            }

            return null;
        }

        public void ClearSolverChanges()
        {
            FoundChange = null;
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
            foreach (var technique in Techniques)
            {
                technique.ClearSolverChanges();
            }
        }
    }

    public class TechniqueWrapper : ITechnique
    {
        public bool IsCollection => false;
        public string Name { get; }
        public ISolverTechnique Technique { get; }
        public IChangeDescription FoundChange { get; private set; }
        public event EventHandler<IChangeDescription> ChangeDescriptionUpdated;

        public TechniqueWrapper(string name, ISolverTechnique technique)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            Technique = technique ?? throw new System.ArgumentNullException(nameof(technique));
        }

        public void ClearSolverChanges()
        {
            FoundChange = null;
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
        }

        public async Task<IChangeDescription> FindChangeFor(BoardState board)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(1, 10)));
            FoundChange = NoChangeDescription.Instance;
            FoundChange = Technique.GetPossibleBoardStateChange(board);
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
            return FoundChange;
        }

        public Task<IChangeDescription> FindNextChangeFor(BoardState board) => FindChangeFor(board);
    }
}