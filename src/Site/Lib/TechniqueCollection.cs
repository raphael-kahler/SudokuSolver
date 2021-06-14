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

    public abstract class BaseTechnique : ITechnique
    {
        public bool IsCollection => false;
        public string Name { get; }
        public IChangeDescription FoundChange { get; private set; }
        public event EventHandler<IChangeDescription> ChangeDescriptionUpdated;

        public BaseTechnique(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
        }

        public void ClearSolverChanges()
        {
            FoundChange = null;
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
        }

        public Task<IChangeDescription> FindNextChangeFor(BoardState board) => FindChangeFor(board);

        public async Task<IChangeDescription> FindChangeFor(BoardState board)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            FoundChange = NoChangeDescription.Instance;
            FoundChange = FindChange(board);
            ChangeDescriptionUpdated?.Invoke(this, FoundChange);
            return FoundChange;
        }

        protected abstract IChangeDescription FindChange(BoardState board);
    }

    public class TechniqueWrapper : BaseTechnique
    {
        private readonly ISolverTechnique technique;

        public TechniqueWrapper(string name, ISolverTechnique technique) : base(name) =>
            this.technique = technique ?? throw new System.ArgumentNullException(nameof(technique));

        protected override IChangeDescription FindChange(BoardState board) =>
            this.technique.GetPossibleBoardStateChange(board);
    }

    public class SolverWrapper : BaseTechnique
    {
        private readonly ISolver solver;

        public SolverWrapper(string name, ISolver solver) : base(name) =>
            this.solver = solver ?? throw new System.ArgumentNullException(nameof(solver));

        protected override IChangeDescription FindChange(BoardState board) =>
            this.solver.GetNextChange(board);
    }
}