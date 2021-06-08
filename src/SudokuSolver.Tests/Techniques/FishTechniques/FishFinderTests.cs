using System.Collections.Generic;
using SudokuSolver.Techniques.FishTechniques;
using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests.Techniques.FishTechniques
{
    public class FishFinderTests
    {
        public static IEnumerable<object[]> FishFinder_FishPatternInputs()
        {
            // no fish
            yield return new object[]
            {
                FishType.NoFish,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (0, 2), (0, 5) },
                    new List<Position> { (2, 4), (2, 8) }
                }
            };
            // x-wing
            yield return new object[]
            {
                FishType.Regular,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (4, 4), (4, 8) },
                    new List<Position> { (2, 4), (2, 8) }
                }
            };
            // sashimi x-wing
            yield return new object[]
            {
                FishType.Sashimi,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (4, 4), (4, 7) },
                    new List<Position> { (2, 4), (2, 8) }
                }
            };
            // sashimi x-wing (reversed row order)
            yield return new object[]
            {
                FishType.Sashimi,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (2, 4), (2, 8) },
                    new List<Position> { (4, 4), (4, 7) }
                }
            };
            // finned x-wing
            yield return new object[]
            {
                FishType.Finned,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (2, 1), (2, 8) },
                    new List<Position> { (4, 1), (4, 2), (4, 8) }
                }
            };
            // finned x-wing (reversed order)
            yield return new object[]
            {
                FishType.Finned,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (4, 1), (4, 2), (4, 8) },
                    new List<Position> { (2, 1), (2, 8) }
                }
            };
            // x-wing + plus fin
            yield return new object[]
            {
                FishType.Finned,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 1), (7, 3), (7, 4) }
                }
            };
            // x-wing + plus two fins
            yield return new object[]
            {
                FishType.Finned,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 1), (7, 3), (7, 4), (7, 5) }
                }
            };
            // no fish
            yield return new object[]
            {
                FishType.NoFish,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 5) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 4), (7, 6) },
                }
            };
            // no fish
            yield return new object[]
            {
                FishType.NoFish,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 5), (1, 6), (1, 7) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 4), (7, 6) },
                }
            };
            // three-fish
            yield return new object[]
            {
                FishType.Regular,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 6) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 4), (7, 6) },
                }
            };
            // sashimi three-fish
            yield return new object[]
            {
                FishType.Sashimi,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 7) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 4), (7, 6) },
                }
            };
            // two-finned sashimi three-fish
            yield return new object[]
            {
                FishType.Sashimi,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 4), (1, 7), (1, 8) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 1), (7, 4), (7, 6) },
                }
            };
            // finned three-fish
            yield return new object[]
            {
                FishType.Finned,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 4), (1, 6), (1, 7) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 4), (7, 6) },
                }
            };
            // two-fin three-fish
            yield return new object[]
            {
                FishType.Finned,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 4), (1, 6), (1, 7), (1, 8) },
                    new List<Position> { (4, 1), (4, 4) },
                    new List<Position> { (7, 1), (7, 4), (7, 6) },
                }
            };
            // no fish
            yield return new object[]
            {
                FishType.NoFish,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (2, 0), (2, 2) },
                    new List<Position> { (3, 0), (3, 2), (3, 3), (3, 6), (3, 8) },
                    new List<Position> { (4, 0), (4, 8) },
                }
            };
            // four-row fish
            yield return new object[]
            {
                FishType.Regular,
                new LargeFishFinder(4, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { (1, 1), (1, 5) },
                    new List<Position> { (2, 7), (2, 5) },
                    new List<Position> { (5, 7), (5, 3) },
                    new List<Position> { (6, 1), (6, 3) },
                }
            };
        }

        [Theory]
        [MemberData(nameof(FishFinder_FishPatternInputs))]
        internal void GetFishType(FishType fishType, IFishFinder fishFinder, IList<IList<Position>> rows)
        {
            var fish = fishFinder.GetFishType(rows);
            Assert.Equal(fishType, fish.FishType);
        }
    }
}