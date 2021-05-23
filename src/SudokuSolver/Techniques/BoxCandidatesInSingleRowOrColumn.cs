using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques
{
    public abstract class BoxCandidatesInSingleRowOrColumn : ISolverTechnique
    {
        public abstract string Description { get; }
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            for (int box = 0; box < 9; ++box)
            {
                var cells = board.Box(box);
                for (int value = 1; value <= 9; ++value)
                {
                    var change = GetChangeForValue(board, cells, value);
                    if (change.HasEffect)
                    {
                        return new BoardStateChangeCandidateRemoval(change.CandidatesAffected, this, change);
                    }
                }
            }
            return new BoardStateNoChange();
        }

        protected abstract IChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value);
    }

    public class BoxCandidatesInSingleRow : BoxCandidatesInSingleRowOrColumn
    {
        public override string Description => "In one box all candidates of a number are in the same row. Remove candidates from other boxes of that row.";

        protected override IChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
        {
            var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
            if (cellsForCandidate.Any())
            {
                var row = cellsForCandidate.First().Position.Row;
                if (cellsForCandidate.All(c => c.Position.Row == row))
                {
                    foreach (var rowCell in board.Row(row))
                    {
                        if (rowCell.Candidates.Contains(value))
                        {
                            var candidate = new Candidate(rowCell.Position, value);
                            if (cellsForCandidate.Contains(rowCell))
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

            return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        }
    }

    public class BoxCandidatesInSingleColumn : BoxCandidatesInSingleRowOrColumn
    {
        public override string Description => "In one box all candidates of a number are in the same column. Remove candidates from other boxes of that column.";

        protected override IChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
        {
            var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
            if (cellsForCandidate.Any())
            {
                var col = cellsForCandidate.First().Position.Col;
                if (cellsForCandidate.All(c => c.Position.Col == col))
                {
                    foreach (var columnCell in board.Column(col))
                    {
                        if (columnCell.Candidates.Contains(value))
                        {
                            Candidate candidate = new Candidate(columnCell.Position, value);
                            if (cellsForCandidate.Contains(columnCell))
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

            return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        }
    }
}