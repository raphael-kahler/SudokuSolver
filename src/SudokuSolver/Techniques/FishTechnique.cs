using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.FishHelpers;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class FishTechnique : ISolverTechnique
    {
        public string Description => $"{Size}-{Orientation.PrimaryDimensionName} Fish.";
        public DifficultyLevel DifficultyLevel => Size < 3 ? DifficultyLevel.Advanced : DifficultyLevel.Expert;
        private int Size { get; }
        private IOrientation Orientation { get; }

        private FishTechnique(int size, IOrientation oritentation)
        {
            Size = size;
            Orientation = oritentation ?? throw new System.ArgumentNullException(nameof(oritentation));
        }

        public static FishTechnique TwoRow() => new FishTechnique(2, RowOrientation.Instance);
        public static FishTechnique TwoColumn() => new FishTechnique(2, ColumnOrientation.Instance);
        public static FishTechnique ThreeRow() => new FishTechnique(3, RowOrientation.Instance);
        public static FishTechnique ThreeColumn() => new FishTechnique(3, ColumnOrientation.Instance);
        public static FishTechnique FourRow() => new FishTechnique(4, RowOrientation.Instance);
        public static FishTechnique FourColumn() => new FishTechnique(4, ColumnOrientation.Instance);

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            for (int value = 1; value <= 9; ++value)
            {
                var changeDescription = GetChangeForValue(board, value);
                if (changeDescription.HasEffect)
                {
                    return new BoardStateChangeCandidateRemoval(changeDescription.CandidatesAffected, this, changeDescription);
                }
            }

            return new BoardStateNoChange();
        }

        private IChangeDescription GetChangeForValue(BoardState board, int value)
        {
            var cellCollections = GetCellCollections(board)
                .Select(cells => cells.Where(c => c.Candidates.Contains(value)).Select(c => c.Position).ToList())
                .ToList();

            var cells = new List<Position>[Size];
            foreach (var permutation in CollectionPermutator.Permutate(cellCollections.Count, Size))
            {
                for (int i = 0; i < Size; ++i)
                {
                    cells[i] = cellCollections[permutation[i]];
                }
                var fish = GetFishFinder().GetFishType(cells);
                var removals = fish.DetermineCandidateRemovals(board, value).ToImmutableHashSet();
                if (removals.Any())
                {
                    var causers = fish.DefiningCandidates(value).ToImmutableHashSet();
                    return ChangeDescription.CandidatesRemovingCandidates(causers, removals);
                }
            }

            return NoChangeDescription.Instance;
        }

        private IList<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            Enumerable.Range(0, 9)
                .Select(value => Orientation.CellsForPrimaryDimension(board, value))
                .ToList();

        private IFishFinder GetFishFinder() => Size == 2 ? new TwoFishFinder(Orientation) : new LargeFishFinder(Size, Orientation);
    }
}