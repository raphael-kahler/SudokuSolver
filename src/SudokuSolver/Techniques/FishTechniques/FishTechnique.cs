using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.FishTechniques
{
    internal class FishTechnique : ISolverTechnique
    {
        public string Description => $"{Size}-{Orientation.PrimaryDimensionName} Fish.";
        public DifficultyLevel DifficultyLevel => Size < 3 ? DifficultyLevel.Advanced : DifficultyLevel.Expert;
        private int Size { get; }
        private IOrientation Orientation { get; }

        internal FishTechnique(int size, IOrientation oritentation)
        {
            Size = size;
            Orientation = oritentation ?? throw new System.ArgumentNullException(nameof(oritentation));
        }

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            for (int value = 1; value <= 9; ++value)
            {
                var change = GetChangeForValue(board, value);
                if (change.HasEffect)
                {
                    var hinter = new FishTechniqueHinter();
                    return new ChangeDescription(change, hinter, this);
                }
            }

            return NoChangeDescription.Instance;
        }

        private IBoardStateChange GetChangeForValue(BoardState board, int value)
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
                    return BoardStateChange.ForCandidatesRemovingCandidates(causers, removals);
                }
            }

            return BoardStateNoChange.Instance;
        }

        private IList<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            Enumerable.Range(0, 9)
                .Select(value => Orientation.CellsForPrimaryDimension(board, value))
                .ToList();

        private IFishFinder GetFishFinder() => Size == 2 ? new TwoFishFinder(Orientation) : new LargeFishFinder(Size, Orientation);
    }

    internal class FishTechniqueHinter : IChangeHinter
    {
        public IEnumerable<ChangeHint> GetHints()
        {
            throw new System.NotImplementedException();
        }
    }
}