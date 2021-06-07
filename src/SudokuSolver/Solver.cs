using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public interface ISolver
    {
        IChangeDescription GetNextChange(BoardState board);
    }

    public class Solver : ISolver
    {
        private bool globChanges;
        private IImmutableList<ISolverTechnique> techniques;

        public Solver() : this(ImmutableList<ISolverTechnique>.Empty, false)
        { }

        public Solver(IImmutableList<ISolverTechnique> techniques, bool globChanges)
        {
            this.techniques = techniques;
            this.globChanges = globChanges;
        }

        public Solver WithTechnique(ISolverTechnique technique) =>
            new Solver(this.techniques.Add(technique), this.globChanges);

        public Solver WithTechnique(IEnumerable<ISolverTechnique> techniques) =>
            new Solver(this.techniques.AddRange(techniques), this.globChanges);

        public Solver GlobChanges() =>
            new Solver(this.techniques, true);

        public IChangeDescription GetNextChange(BoardState board)
        {
            var globbedChange = GetGlobbedChange(board);
            if (globbedChange.Change.HasEffect)
            {
                return globbedChange;
            }

            foreach (var technique in this.techniques)
            {
                var changeDescription = technique.GetPossibleBoardStateChange(board);
                if (changeDescription.Change.HasEffect)
                {
                    return changeDescription;
                }
            }
            return NoChangeDescription.Instance;
        }

        private IChangeDescription GetGlobbedChange(BoardState board)
        {
            var changes = new List<IChangeDescription>();
            if (!globChanges)
            {
                return NoChangeDescription.Instance;
            }
            foreach (var technique in this.techniques)
            {
                while (true)
                {
                    var changeDescription = technique.GetPossibleBoardStateChange(board);
                    if (!changeDescription.Change.HasEffect)
                    {
                        break;
                    }
                    changes.Add(changeDescription);
                    board = board.ApplyChange(changeDescription.Change);
                }
            }
            if (changes.Any())
            {
                return new CombinationChangeDescription(changes);
            }
            else
            {
                return NoChangeDescription.Instance;
            }
        }
    }

    public class ChainedSolver : ISolver
    {
        private IImmutableList<ISolver> solvers;

        public ChainedSolver() : this(ImmutableList<ISolver>.Empty)
        { }

        public ChainedSolver(IImmutableList<ISolver> solvers) => this.solvers = solvers;

        public ChainedSolver WithSolver(ISolver solver) =>
            new ChainedSolver(this.solvers.Add(solver));

        public ChainedSolver WithSolver(IEnumerable<ISolver> solvers) =>
            new ChainedSolver(this.solvers.AddRange(solvers));

        public IChangeDescription GetNextChange(BoardState board)
        {
            foreach (var solver in this.solvers)
            {
                var changeDescription = solver.GetNextChange(board);
                if (changeDescription.Change.HasEffect)
                {
                    return changeDescription;
                }
            }
            return NoChangeDescription.Instance;
        }
    }
}