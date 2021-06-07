using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    internal class EliminationByValueTechnique : CollectionCandidateRemover
    {
        private readonly ICellCollector cellCollector;
        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Trivial;
        public override string Description => $"Each number is only allowed once per {this.cellCollector.CollectionName}.";

        internal EliminationByValueTechnique(ICellCollector cellCollector)
        {
            this.cellCollector = cellCollector;
        }

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            this.cellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var valuesCausingChange = ImmutableHashSet<Position>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;


            var cellsWithValue = cells.Where(c => c.Value.HasValue);
            foreach (var valueCell in cellsWithValue)
            {
                var value = valueCell.Value.Value;
                foreach (var cell in cells)
                {
                    if (cell.Candidates.Contains(value))
                    {
                        valuesCausingChange = valuesCausingChange.Add(valueCell.Position);
                        candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, value));
                    }
                }
            }

            var change = BoardStateChange.ForValuesRemovingCandidates(valuesCausingChange, candidatesToRemove);
            return new ChangeDescription(change, NoHints.Instance, this);
        }

    }
}