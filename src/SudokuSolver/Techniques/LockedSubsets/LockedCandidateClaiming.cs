using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.LockedSubsets
{
    internal class LockedCandidatesClaimingTechnique : ISolverTechnique
    {
        private readonly ICellCollector cellCollector;
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public string Description =>
            $"In one {cellCollector.CollectionName} all candidates of a number are in the same box. " +
            $"Remove candidates from other {cellCollector.CollectionName}s of that box.";

        internal LockedCandidatesClaimingTechnique(ICellCollector cellCollector)
        {
            this.cellCollector = cellCollector ?? throw new System.ArgumentNullException(nameof(cellCollector));
        }

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            var candidatesToRemove = new List<Candidate>();
            foreach (var cellCollection in cellCollector.GetCollections(board))
            {
                for (int value = 1; value <= 9; ++value)
                {
                    var change = GetChangeForValue(board, cellCollection, value);
                    if (change.HasEffect)
                    {
                        return new ChangeDescription(change, NoHints.Instance, this);
                    }
                }
            }

            return NoChangeDescription.Instance;
        }

        private IBoardStateChange GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
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

            return BoardStateChange.ForCandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        }
    }
}