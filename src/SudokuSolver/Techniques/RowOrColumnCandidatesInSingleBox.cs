using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public abstract class CollectionCandidatesInSingleBox : ISolverTechnique
    {
        public abstract string Description { get; }
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        protected abstract IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board);

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var candidatesToRemove = new List<Candidate>();
            foreach (var cellCollection in GetCellCollections(board))
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

            return new BoardStateNoChange();
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

            return ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        }
    }

    public class RowCandidatesInSingleBox : CollectionCandidatesInSingleBox
    {
        public override string Description => "In one row all candidates of a number are in the same box. Remove candidates from other rows of that box.";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollections.GetRows(board);
    }

    public class ColumnCandidatesInSingleBox : CollectionCandidatesInSingleBox
    {
        public override string Description => "In one column all candidates of a number are in the same box. Remove candidates from other columns of that box.";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollections.GetColumns(board);
   }
}