using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class CellsForCandidates : CollectionCandidateRemover
    {
        private record CellsForCandidate(int CandidateValue, IReadOnlyList<Cell> Cells);
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => this.size == 1 ? DifficultyLevel.Easy : DifficultyLevel.Medium;

        public override string Description =>
            $"{this.size} number{(this.size > 1 ? "s are" : " is")} the candidate of only {this.size} " +
            $"cell{(this.size > 1 ? "s" : string.Empty)} in a {this.cellCollector.CollectionName}. All other candidates of the cells can be removed.";

        internal CellsForCandidates(int size, ICellCollector cellCollector)
        {
            this.size = size;
            this.cellCollector = cellCollector;
        }
        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) => this.cellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var cellsForCandidates = Enumerable.Range(1, 9)
                .Select(value => new CellsForCandidate(value, cells.Where(c => c.Candidates.Contains(value)).ToList()))
                .Where(c => c.Cells.Count > 0 && c.Cells.Count <= this.size)
                .ToList();

            if (cellsForCandidates.Count < this.size)
            {
                return NoChangeDescription.Instance;
            }

            foreach(var combination in CollectionPermutator.Permutate(cellsForCandidates.Count, this.size))
            {
                var cellsForCombination = combination.SelectMany(idx => cellsForCandidates[idx].Cells.Select(c => c.Position)).ToHashSet();
                if (cellsForCombination.Count == this.size)
                {
                    var candidatesInCombination = combination.Select(idx => cellsForCandidates[idx].CandidateValue);
                    var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
                    var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

                    foreach(var cell in cells.Where(c => cellsForCombination.Contains(c.Position)))
                    {
                        foreach (var candidate in cell.Candidates)
                        {
                            var candidateToRecord = new Candidate(cell.Position, candidate);
                            if (candidatesInCombination.Contains(candidate))
                            {
                                candidatesCausingChange = candidatesCausingChange.Add(candidateToRecord);
                            }
                            else
                            {
                                candidatesToRemove = candidatesToRemove.Add(candidateToRecord);
                            }
                        }
                    }

                    return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                }
            };

            return NoChangeDescription.Instance;

        }

        public static CellsForCandidates OnePerRow() => new CellsForCandidates(1, RowCellCollector.Instance);
        public static CellsForCandidates OnePerColumn() => new CellsForCandidates(1, ColumnCellCollector.Instance);
        public static CellsForCandidates OnePerBox() => new CellsForCandidates(1, BoxCellCollector.Instance);
        public static CellsForCandidates TwoPerRow() => new CellsForCandidates(2, RowCellCollector.Instance);
        public static CellsForCandidates TwoPerColumn() => new CellsForCandidates(2, ColumnCellCollector.Instance);
        public static CellsForCandidates TwoPerBox() => new CellsForCandidates(2, BoxCellCollector.Instance);
        public static CellsForCandidates ThreePerRow() => new CellsForCandidates(3, RowCellCollector.Instance);
        public static CellsForCandidates ThreePerColumn() => new CellsForCandidates(3, ColumnCellCollector.Instance);
        public static CellsForCandidates ThreePerBox() => new CellsForCandidates(3, BoxCellCollector.Instance);
        public static CellsForCandidates FourPerRow() => new CellsForCandidates(4, RowCellCollector.Instance);
        public static CellsForCandidates FourPerColumn() => new CellsForCandidates(4, ColumnCellCollector.Instance);
        public static CellsForCandidates FourPerBox() => new CellsForCandidates(4, BoxCellCollector.Instance);
    }
}