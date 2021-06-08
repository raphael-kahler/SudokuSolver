using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    internal abstract class CollectionCandidateRemover : ISolverTechnique
    {
        public abstract string Description { get; }
        public abstract DifficultyLevel DifficultyLevel { get; }
        protected abstract IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board);
        protected abstract IChangeDescription FindChange(IEnumerable<Cell> cells);

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            foreach (var cellCollection in GetCellCollections(board))
            {
                var changeDescription = FindChange(cellCollection);
                if (changeDescription.Change.HasEffect)
                {
                    return changeDescription;
                }
            }

            return NoChangeDescription.Instance;
        }
    }
}