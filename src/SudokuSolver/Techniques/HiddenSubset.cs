using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class HiddenSubset : CollectionCandidateRemover
    {
        private record CellsForCandidate(int CandidateValue, IReadOnlyList<Cell> Cells);
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => this.size == 1 ? DifficultyLevel.Easy : DifficultyLevel.Medium;

        public override string Description =>
            $"{this.size} number{(this.size > 1 ? "s are" : " is")} the candidate of only {this.size} " +
            $"cell{(this.size > 1 ? "s" : string.Empty)} in a {this.cellCollector.CollectionName}. All other candidates of the cell{(this.size > 1 ? "s" : string.Empty)} can be removed.";

        internal HiddenSubset(int size, ICellCollector cellCollector)
        {
            this.size = size;
            this.cellCollector = cellCollector;
        }
        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) => this.cellCollector.GetCollections(board);

        protected override IBoardStateChange FindChange(IEnumerable<Cell> cells)
        {
            var cellsForCandidates = Enumerable.Range(1, 9)
                .Select(value => new CellsForCandidate(value, cells.Where(c => c.Candidates.Contains(value)).ToList()))
                .Where(c => c.Cells.Count > 0 && c.Cells.Count <= this.size)
                .ToList();

            if (cellsForCandidates.Count < this.size)
            {
                return BoardStateNoChange.Instance;
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

                    return BoardStateChange.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                }
            };

            return BoardStateNoChange.Instance;

        }

        public static HiddenSubset HiddenSingleRow() => new HiddenSubset(1, RowCellCollector.Instance);
        public static HiddenSubset HiddenSingleColumn() => new HiddenSubset(1, ColumnCellCollector.Instance);
        public static HiddenSubset HiddenSingleBox() => new HiddenSubset(1, BoxCellCollector.Instance);
        public static HiddenSubset HiddenPairRow() => new HiddenSubset(2, RowCellCollector.Instance);
        public static HiddenSubset HiddenPairColumn() => new HiddenSubset(2, ColumnCellCollector.Instance);
        public static HiddenSubset HiddenPairBox() => new HiddenSubset(2, BoxCellCollector.Instance);
        public static HiddenSubset HiddenTripleRow() => new HiddenSubset(3, RowCellCollector.Instance);
        public static HiddenSubset HiddenTripleColumn() => new HiddenSubset(3, ColumnCellCollector.Instance);
        public static HiddenSubset HiddenTripleBox() => new HiddenSubset(3, BoxCellCollector.Instance);
        public static HiddenSubset HiddenQuadRow() => new HiddenSubset(4, RowCellCollector.Instance);
        public static HiddenSubset HiddenQuadColumn() => new HiddenSubset(4, ColumnCellCollector.Instance);
        public static HiddenSubset HiddenQuadBox() => new HiddenSubset(4, BoxCellCollector.Instance);
        public static IEnumerable<HiddenSubset> HiddenSingles() => new List<HiddenSubset> { HiddenSingleRow(), HiddenSingleColumn(), HiddenSingleBox() };
        public static IEnumerable<HiddenSubset> HiddenPairs() => new List<HiddenSubset> { HiddenPairRow(), HiddenPairColumn(), HiddenPairBox() };
        public static IEnumerable<HiddenSubset> HiddenTriples() => new List<HiddenSubset> { HiddenTripleRow(), HiddenTripleColumn(), HiddenTripleBox() };
        public static IEnumerable<HiddenSubset> HiddenQuads() => new List<HiddenSubset> { HiddenQuadRow(), HiddenQuadColumn(), HiddenQuadBox() };
    }

    internal class HiddenSubsetHinter : IChangeHinter
    {
        private string techniqueName;
        private ICellCollector cellCollector;
        private Position position;

        public IEnumerable<ChangeHint> GetHints()
        {
            yield return new ChangeHint($"Find a {techniqueName} in a {cellCollector.CollectionName}");
            yield return new ChangeHint($"It is in {this.cellCollector.CollectionName} {this.cellCollector.Indexer.CollectionIndex(this.position)}");
        }
    }
}