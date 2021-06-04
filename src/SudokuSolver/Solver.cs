using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public interface ISolver
    {
        IBoardStateChange GetNextChange(BoardState board);
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

        public IBoardStateChange GetNextChange(BoardState board)
        {
            var globbedChange = GetGlobbedChange(board);
            if (globbedChange.CausesChange)
            {
                return globbedChange;
            }

            foreach (var technique in this.techniques)
            {
                var change = technique.GetPossibleBoardStateChange(board);
                if (change.CausesChange)
                {
                    return change;
                }
            }
            return BoardStateNoChange.Instance;
        }

        private IBoardStateChange GetGlobbedChange(BoardState board)
        {
            var changes = new List<IBoardStateChange>();
            if (!globChanges)
            {
                return BoardStateNoChange.Instance;
            }
            foreach (var technique in this.techniques)
            {
                while (true)
                {
                    var change = technique.GetPossibleBoardStateChange(board);
                    if (!change.CausesChange)
                    {
                        break;
                    }
                    changes.Add(change);
                    board = board.ApplyChange(change);
                }
            }
            if (changes.Any())
            {
                return new BoardStateChangeCombination(changes);
            }
            else
            {
                return BoardStateNoChange.Instance;
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

        public IBoardStateChange GetNextChange(BoardState board)
        {
            foreach (var solver in this.solvers)
            {
                var change = solver.GetNextChange(board);
                if (change.CausesChange)
                {
                    return change;
                }
            }
            return BoardStateNoChange.Instance;
        }
    }
}