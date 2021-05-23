using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class CellsWithSameCandidatesInCollection : CollectionCandidateRemover
    {
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public override string Description =>
            $"{this.size} cells in a {this.cellCollector.CollectionName} have only the same {this.size} candidates. " +
            $"Those candidates can be removed from all other cells in the {this.cellCollector.CollectionName}.";

        internal CellsWithSameCandidatesInCollection(int size, ICellCollector cellCollector)
        {
            this.size = size;
            this.cellCollector = cellCollector;
        }

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            this.cellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var candidatesCausingChange = new HashSet<Candidate>();
            var candidatesToRemove = new HashSet<Candidate>();

            var cellsWithTwoCandidates = cells.Where(c => c.Candidates.Count == this.size).ToList();
            for (int i = 0; i < cellsWithTwoCandidates.Count - 1; ++i)
            {
                var candidates = cellsWithTwoCandidates[i].Candidates;
                var numMatchingCells = cellsWithTwoCandidates.Skip(i + 1).Count(c => c.Candidates.SetEquals(candidates));
                if (numMatchingCells == this.size - 1)
                {
                    foreach (var cell in cells)
                    {
                        if (cell.Candidates.SetEquals(candidates))
                        {
                            foreach (var candidate in candidates)
                            {
                                candidatesCausingChange.Add(new Candidate(cell.Position, candidate));
                            }
                        }
                        else
                        {
                            foreach (var candidate in candidates)
                            {
                                if (cell.Candidates.Contains(candidate))
                                {
                                    candidatesToRemove.Add(new Candidate(cell.Position, candidate));
                                }
                            }
                        }
                    }
                }
            }

            return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange.ToImmutableHashSet(), candidatesToRemove.ToImmutableHashSet());
       }

       public static CellsWithSameCandidatesInCollection TwoPerRow() => new CellsWithSameCandidatesInCollection(2, RowCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection TwoPerColumn() => new CellsWithSameCandidatesInCollection(2, ColumnCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection TwoPerBox() => new CellsWithSameCandidatesInCollection(2, BoxCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection ThreePerRow() => new CellsWithSameCandidatesInCollection(3, RowCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection ThreePerColumn() => new CellsWithSameCandidatesInCollection(3, ColumnCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection ThreePerBox() => new CellsWithSameCandidatesInCollection(3, BoxCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection FourPerRow() => new CellsWithSameCandidatesInCollection(4, RowCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection FourPerColumn() => new CellsWithSameCandidatesInCollection(4, ColumnCellCollector.Instance);
       public static CellsWithSameCandidatesInCollection FourPerBox() => new CellsWithSameCandidatesInCollection(4, BoxCellCollector.Instance);
    }
}