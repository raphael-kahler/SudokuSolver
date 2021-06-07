using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    public abstract class CollectionCandidateRemover : ISolverTechnique
    {
        public abstract string Description { get; }
        public abstract DifficultyLevel DifficultyLevel { get; }
        protected abstract IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board);
        protected abstract IBoardStateChange FindChange(IEnumerable<Cell> cells);

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            foreach (var cellCollection in GetCellCollections(board))
            {
                var change = FindChange(cellCollection);
                if (change.HasEffect)
                {
                    return new ChangeDescription(change, NoHints.Instance, this);
                }
            }

            return NoChangeDescription.Instance;
        }
    }
}