using System.Collections.Generic;
using System.Collections.Immutable;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public class Solver
    {
        private IImmutableList<ISolverTechnique> techniques;

        public Solver() : this(ImmutableList<ISolverTechnique>.Empty)
        { }

        public Solver(IImmutableList<ISolverTechnique> techniques) =>
            this.techniques = techniques;

        public Solver WithTechnique(ISolverTechnique technique) =>
            new Solver(this.techniques.Add(technique));

        public Solver WithTechnique(IEnumerable<ISolverTechnique> techniques) =>
            new Solver(this.techniques.AddRange(techniques));

        public IBoardStateChange GetNextChange(BoardState board)
        {
            foreach (var technique in this.techniques)
            {
                var change = technique.GetPossibleBoardStateChange(board);
                if (change.CausesChange)
                {
                    return change;
                }
            }
            return new BoardStateNoChange();
        }
    }
}