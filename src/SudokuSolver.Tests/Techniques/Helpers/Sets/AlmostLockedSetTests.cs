using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers.Sets;
using Xunit;

namespace SudokuSolver.Tests.Techniques.Helpers.Sets
{
    public class AlmostLockedSetTests
    {
        public static IEnumerable<object[]> FormsWxyzWingWith_RowPattern_TestInputs()
        {
            yield return new object[] { new Cell((1, 1), 1, 2), true, 2 };
            yield return new object[] { new Cell((1, 1), 1, 3), true, 3 };
            yield return new object[] { new Cell((1, 1), 1, 4), true, 4 };
            yield return new object[] { new Cell((1, 1), 1, 5), false, 0 };
            yield return new object[] { new Cell((1, 1), 2, 3), false, 0 };
            yield return new object[] { new Cell((2, 7), 1, 2), false, 0 };
            yield return new object[] { new Cell((0, 0), 1, 2), false, 0 };
        }

        [Theory]
        [MemberData(nameof(FormsWxyzWingWith_RowPattern_TestInputs))]
        public void FormsWxyzWingWith_RowPattern(Cell cell, bool shouldBeWing, int zValue)
        {
            var set = new AlmostLockedSet(new List<Cell>
            {
                new Cell((0, 2), 1, 2, 3, 4),
                new Cell((0, 4), 2, 3, 4),
                new Cell((0, 6), 2, 3, 4),
            });
            AssertFormsWxyzWith(set, cell, shouldBeWing, zValue);
        }

        public static IEnumerable<object[]> FormsWxyzWingWith_ColumnPattern_TestInputs()
        {
            yield return new object[] { new Cell((1, 1), 1, 2), true, 2 };
            yield return new object[] { new Cell((1, 1), 1, 3), true, 3 };
            yield return new object[] { new Cell((1, 1), 1, 4), true, 4 };
            yield return new object[] { new Cell((1, 1), 1, 5), false, 0 };
            yield return new object[] { new Cell((1, 1), 2, 3), false, 0 };
            yield return new object[] { new Cell((7, 2), 1, 2), false, 0 };
            yield return new object[] { new Cell((0, 0), 1, 2), false, 0 };
        }

        [Theory]
        [MemberData(nameof(FormsWxyzWingWith_ColumnPattern_TestInputs))]
        public void FormsWxyzWingWith_ColumnPattern(Cell cell, bool shouldBeWing, int zValue)
        {
            var set = new AlmostLockedSet(new List<Cell>
            {
                new Cell((2, 0), 1, 2, 3, 4),
                new Cell((4, 0), 2, 3, 4),
                new Cell((6, 0), 2, 3, 4),
            });
            AssertFormsWxyzWith(set, cell, shouldBeWing, zValue);
        }

        public static IEnumerable<object[]> FormsWxyzWingWith_BoxPattern_TestInputs()
        {
            yield return new object[] { new Cell((0, 6), 1, 2), true, 2 };
            yield return new object[] { new Cell((0, 6), 1, 3), true, 3 };
            yield return new object[] { new Cell((0, 6), 1, 4), true, 4 };
            yield return new object[] { new Cell((0, 6), 1, 5), false, 0 };
            yield return new object[] { new Cell((0, 6), 2, 3), false, 0 };
            yield return new object[] { new Cell((1, 6), 1, 2), false, 0 };
            yield return new object[] { new Cell((2, 6), 1, 2), false, 0 };
            yield return new object[] { new Cell((6, 2), 1, 2), true, 2 };
            yield return new object[] { new Cell((6, 2), 1, 3), true, 3 };
            yield return new object[] { new Cell((6, 2), 1, 4), true, 4 };
            yield return new object[] { new Cell((6, 2), 1, 5), false, 0 };
            yield return new object[] { new Cell((6, 2), 2, 3), false, 0 };
            yield return new object[] { new Cell((6, 0), 1, 2), false, 0 };
            yield return new object[] { new Cell((6, 1), 1, 2), false, 0 };
            yield return new object[] { new Cell((0, 0), 1, 2), false, 0 };
        }

        [Theory]
        [MemberData(nameof(FormsWxyzWingWith_BoxPattern_TestInputs))]
        public void FormsWxyzWingWith_BoxPattern(Cell cell, bool shouldBeWing, int zValue)
        {
            var set = new AlmostLockedSet(new List<Cell>
            {
                new Cell((0, 2), 1, 2, 3, 4),
                new Cell((1, 0), 2, 3, 4),
                new Cell((2, 1), 2, 3, 4),
            });
            AssertFormsWxyzWith(set, cell, shouldBeWing, zValue);
        }

        public static IEnumerable<object[]> FormsWxyzWingWith_BoxPattern_TwoWCells_TestInputs()
        {
            yield return new object[] { new Cell((0, 6), 5, 8), true, 8 };
            yield return new object[] { new Cell((0, 6), 4, 8), true, 8 };
            yield return new object[] { new Cell((0, 6), 5, 9), false, 0 };
            yield return new object[] { new Cell((1, 6), 5, 8), false, 0 };
            yield return new object[] { new Cell((5, 1), 5, 8), false, 0 };
            yield return new object[] { new Cell((5, 2), 5, 8), false, 0 };
        }

        [Theory]
        [MemberData(nameof(FormsWxyzWingWith_BoxPattern_TwoWCells_TestInputs))]
        public void FormsWxyzWingWith_BoxPattern_TwoWCells(Cell cell, bool shouldBeWing, int zValue)
        {
            var set = new AlmostLockedSet(new List<Cell>
            {
                new Cell((0, 1), 4, 5, 8),
                new Cell((0, 2), 4, 5),
                new Cell((2, 0), 8, 9),
            });
            AssertFormsWxyzWith(set, cell, shouldBeWing, zValue);
        }

        private static void AssertFormsWxyzWith(AlmostLockedSet set, Cell cell, bool shouldBeWing, int zValue)
        {
            var maybeWing = set.FormsWxyzWingWith(cell);
            Assert.Equal(shouldBeWing, maybeWing.Is);
            if (shouldBeWing)
            {
                Assert.Equal(zValue, maybeWing.Value.ZValue);
            }
        }
    }
}