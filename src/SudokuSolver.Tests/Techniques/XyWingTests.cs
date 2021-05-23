using System.Collections.Immutable;
using SudokuSolver.Techniques;
using Xunit;

namespace SudokuSolver.Tests.Techniques
{
    public class XyWingTests
    {
        [Theory]
        [InlineData(1, 2, 1, true)]
        [InlineData(1, 2, 2, false)]
        [InlineData(1, 2, 5, false)]
        [InlineData(2, 8, 1, true)]
        [InlineData(2, 8, 2, false)]
        [InlineData(2, 8, 5, false)]
        [InlineData(0, 2, 1, false)]
        [InlineData(1, 4, 1, false)]
        public void Test(int row, int col, int value, bool shouldApply)
        {
            var xyWing = new XyWing(
                new Cell(new Position(1, 1), null, ImmutableHashSet<int>.Empty.Add(2).Add(5)),
                new Cell(new Position(1, 8), null, ImmutableHashSet<int>.Empty.Add(1).Add(2)),
                new Cell(new Position(2, 2), null, ImmutableHashSet<int>.Empty.Add(1).Add(5)));

            var cell = new Cell(new Position(row, col), null, ImmutableHashSet<int>.Empty.Add(value));

            Assert.Equal(shouldApply, xyWing.AppliesTo(cell));
        }
    }
}