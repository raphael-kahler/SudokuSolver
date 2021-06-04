using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    public abstract class CollectionCandidateRemover : ISolverTechnique
    {
        public abstract string Description { get; }
        public abstract DifficultyLevel DifficultyLevel { get; }
        protected abstract IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board);
        protected abstract IChangeDescription FindChange(IEnumerable<Cell> cells);

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            foreach (var cellCollection in GetCellCollections(board))
            {
                var changeDescription = FindChange(cellCollection);
                if (changeDescription.HasEffect)
                {
                    return new BoardStateChangeCandidateRemoval(
                        CandidatesToRemove: changeDescription.CandidatesAffected,
                        FoundBy: this,
                        Description: changeDescription);
                }
            }

            return BoardStateNoChange.Instance;
        }
    }
}