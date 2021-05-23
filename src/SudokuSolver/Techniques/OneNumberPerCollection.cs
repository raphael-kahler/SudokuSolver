using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public abstract class OneNumberPerCollection : CollectionCandidateRemover
    {
        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Trivial;
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

            return ChangeDescription.ValuesRemovingCandidates(valuesCausingChange, candidatesToRemove);
        }
    }

    public class OneNumberPerRow : OneNumberPerCollection
    {
        public override string Description => "Each number is only allowed once per row.";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollections.GetRows(board);
    }

    public class OneNumberPerColumn : OneNumberPerCollection
    {
        public override string Description => "Each number is only allowed once per column.";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollections.GetColumns(board);
    }

    public class OneNumberPerBox : OneNumberPerCollection
    {
        public override string Description => "Each number is only allowed once per 3x3 box.";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollections.GetBoxes(board);
    }
}