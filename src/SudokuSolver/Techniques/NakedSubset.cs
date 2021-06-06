using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class NakedSingle : ISolverTechnique
    {
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Trivial;
        public string Description => "A cell has only one candidate.";

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var match = board.Cells.FirstOrDefault(c => !c.Value.HasValue && c.Candidates.Count == 1);
            return match != null
                ? ConstructChange(match)
                : BoardStateNoChange.Instance;
        }

        private BoardStateChangeSetNumber ConstructChange(Cell match)
        {
            var candidate = match.Candidates.Single();
            var changeExplanation = ChangeDescription.ValueSetter(
                candidatesCausingChange: ImmutableHashSet<Candidate>.Empty.Add(new Candidate(match.Position, candidate)),
                valueAffected: new Cell(match.Position, candidate, ImmutableHashSet<int>.Empty));

            return new BoardStateChangeSetNumber(match.Position, candidate, this, changeExplanation);
        }
    }

    public class NakedSubset : CollectionCandidateRemover
    {
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public override string Description =>
            $"{this.size} cells in a {this.cellCollector.CollectionName} have only the same {this.size} candidates. " +
            $"Those candidates can be removed from all other cells in the {this.cellCollector.CollectionName}.";

        internal NakedSubset(int size, ICellCollector cellCollector)
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

            return ChangeDescription.CandidatesRemovingCandidates(
                candidatesCausingChange.ToImmutableHashSet(),
                candidatesToRemove.ToImmutableHashSet());
        }

        public static NakedSubset NakedPairRow() => new NakedSubset(2, RowCellCollector.Instance);
        public static NakedSubset NakedPairColumn() => new NakedSubset(2, ColumnCellCollector.Instance);
        public static NakedSubset NakedPairBox() => new NakedSubset(2, BoxCellCollector.Instance);
        public static NakedSubset NakedTripleRow() => new NakedSubset(3, RowCellCollector.Instance);
        public static NakedSubset NakedTripleColumn() => new NakedSubset(3, ColumnCellCollector.Instance);
        public static NakedSubset NakedTripleBox() => new NakedSubset(3, BoxCellCollector.Instance);
        public static NakedSubset NakedQuadRow() => new NakedSubset(4, RowCellCollector.Instance);
        public static NakedSubset NakedQuadColumn() => new NakedSubset(4, ColumnCellCollector.Instance);
        public static NakedSubset NakedQuadBox() => new NakedSubset(4, BoxCellCollector.Instance);
        public static NakedSingle NakedSingle() => new NakedSingle();
        public static IEnumerable<NakedSubset> NakedPairs() => new List<NakedSubset> { NakedPairRow(), NakedPairColumn(), NakedPairBox() };
        public static IEnumerable<NakedSubset> NakedTriples() => new List<NakedSubset> { NakedTripleRow(), NakedTripleColumn(), NakedTripleBox() };
        public static IEnumerable<NakedSubset> NakedQuads() => new List<NakedSubset> { NakedQuadRow(), NakedQuadColumn(), NakedQuadBox() };
    }
}