using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public class Solver
    {
        private bool globTrivialChanges;
        private IImmutableList<ISolverTechnique> techniques;

        public Solver() : this(ImmutableList<ISolverTechnique>.Empty, false)
        { }

        public Solver(IImmutableList<ISolverTechnique> techniques, bool globTrivialChanges)
        {
            this.techniques = techniques;
            this.globTrivialChanges = globTrivialChanges;
        }

        public Solver WithTechnique(ISolverTechnique technique)
        {
            this.techniques = this.techniques.Add(technique);
            return this;
        }

        public Solver WithTechnique(IEnumerable<ISolverTechnique> techniques)
        {
            this.techniques = this.techniques.AddRange(techniques);
            return this;
        }

        public Solver GlobTrivialChanges()
        {
            this.globTrivialChanges = true;
            return this;
        }

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
            if (!globTrivialChanges)
            {
                return BoardStateNoChange.Instance;
            }
            foreach (var technique in this.techniques.Where(t => t.DifficultyLevel ==  DifficultyLevel.Trivial))
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
}