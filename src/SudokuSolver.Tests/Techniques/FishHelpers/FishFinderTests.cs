using System.Collections.Generic;
using SudokuSolver.Techniques.FishHelpers;
using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests.Techniques.FishHelpers
{
    public class FishFinderTests
    {
        public static IEnumerable<object[]> FishFinder_FishPatternInputs()
        {
            // no fish
            yield return new object[]
            {
                false,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(0, 2), new(0, 5) },
                    new List<Position> { new(2, 4), new(2, 8) }
                }
            };
            // x-wing
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(4, 4), new(4, 8) },
                    new List<Position> { new(2, 4), new(2, 8) }
                }
            };
            // sashimi x-wing
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(4, 4), new(4, 7) },
                    new List<Position> { new(2, 4), new(2, 8) }
                }
            };
            // sashimi x-wing (reversed row order)
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(2, 4), new(2, 8) },
                    new List<Position> { new(4, 4), new(4, 7) }
                }
            };
            // two-finned sashimi x-wing
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(2, 1), new(2, 8) },
                    new List<Position> { new(4, 1), new(4, 2), new(4, 8) }
                }
            };
            // two-finned sashimi x-wing (reversed order)
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(4, 1), new(4, 2), new(4, 8) },
                    new List<Position> { new(2, 1), new(2, 8) }
                }
            };
            // x-wing + plus fin
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 1), new(7, 3), new(7, 4) }
                }
            };
            // x-wing + plus two fins
            yield return new object[]
            {
                true,
                new TwoFishFinder(RowOrientation.Instance),
                new List<IList<Position>>
                {
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 1), new(7, 3), new(7, 4), new(7, 5) }
                }
            };
            // no fish
            yield return new object[]
            {
                false,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 5) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 4), new(7, 6) },
                }
            };
            // no fish
            yield return new object[]
            {
                false,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 5), new(1, 6), new(1, 7) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 4), new(7, 6) },
                }
            };
            // three-fish
            yield return new object[]
            {
                true,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 6) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 4), new(7, 6) },
                }
            };
            // finned three-fish
            yield return new object[]
            {
                true,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 7) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 4), new(7, 6) },
                }
            };
            // two-finned three-fish
            yield return new object[]
            {
                true,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 4), new(1, 7), new(1, 8) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 1), new(7, 4), new(7, 6) },
                }
            };
            // three-fish + extra fin
            yield return new object[]
            {
                true,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 4), new(1, 6), new(1, 7) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 4), new(7, 6) },
                }
            };
            // three-fish + extra two-fin
            yield return new object[]
            {
                true,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 4), new(1, 6), new(1, 7), new(1, 8) },
                    new List<Position> { new(4, 1), new(4, 4) },
                    new List<Position> { new(7, 1), new(7, 4), new(7, 6) },
                }
            };
            // no fish
            yield return new object[]
            {
                false,
                new LargeFishFinder(3, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(2, 0), new(2, 2) },
                    new List<Position> { new(3, 0), new(3, 2), new(3, 3), new(3, 6), new(3, 8) },
                    new List<Position> { new(4, 0), new(4, 8) },
                }
            };
            // four-row fish
            yield return new object[]
            {
                true,
                new LargeFishFinder(4, RowOrientation.Instance),
                new List<IList<Position>> {
                    new List<Position> { new(1, 1), new(1, 5) },
                    new List<Position> { new(2, 7), new(2, 5) },
                    new List<Position> { new(5, 7), new(5, 3) },
                    new List<Position> { new(6, 1), new(6, 3) },
                }
            };
        }

        [Theory]
        [MemberData(nameof(FishFinder_FishPatternInputs))]
        internal void GetFishType(bool isFish, IFishFinder fishFinder, IList<IList<Position>> rows)
        {
            var fish = fishFinder.GetFishType(rows);
            Assert.Equal(isFish, fish.IsFish);
        }
    }
}