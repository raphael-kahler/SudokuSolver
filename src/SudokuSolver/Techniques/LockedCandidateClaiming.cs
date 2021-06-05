using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class LockedCandidateClaiming : ISolverTechnique
    {
        private readonly ICellCollector cellCollector;
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public string Description =>
            $"In one {this.cellCollector.CollectionName} all candidates of a number are in the same box. " +
            $"Remove candidates from other {this.cellCollector.CollectionName}s of that box.";

        internal LockedCandidateClaiming(ICellCollector cellCollector)
        {
            this.cellCollector = cellCollector ?? throw new System.ArgumentNullException(nameof(cellCollector));
        }

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var candidatesToRemove = new List<Candidate>();
            foreach (var cellCollection in this.cellCollector.GetCollections(board))
            {
                for (int value = 1; value <= 9; ++value)
                {
                    var change = GetChangeForValue(board, cellCollection, value);
                    if (change.HasEffect)
                    {
                        return new BoardStateChangeCandidateRemoval(change.CandidatesAffected, this, change);
                    }
                }
            }

            return BoardStateNoChange.Instance;
        }

        private IChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
        {
            var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
            if (cellsForCandidate.Any())
            {
                var boxId = cellsForCandidate.First().Position.Box;
                if (cellsForCandidate.All(c => c.Position.Box == boxId))
                {
                    foreach (var boxCell in board.Box(boxId))
                    {
                        if (boxCell.Candidates.Contains(value))
                        {
                            Candidate candidate = new Candidate(boxCell.Position, value);
                            if (cellsForCandidate.Contains(boxCell))
                            {
                                candidatesCausingChange = candidatesCausingChange.Add(candidate);
                            }
                            else
                            {
                                candidatesToRemove = candidatesToRemove.Add(candidate);
                            }
                        }
                    }
                }
            }

            return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove, NoHints.Instance);
        }

        public static LockedCandidateClaiming Row() => new LockedCandidateClaiming(RowCellCollector.Instance);
        public static LockedCandidateClaiming Column() => new LockedCandidateClaiming(ColumnCellCollector.Instance);
        public static IEnumerable<LockedCandidateClaiming> AllDirections() => new List<LockedCandidateClaiming> { Row(), Column() };
    }
}