using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SudokuSolver.Tests
{
    public class BoardStateTests
    {
        [Fact]
        public void Constructor_WrongNumberOfCells()
        {
            var board = BoardFactory.EmptyBoard();

            Assert.Throws<ArgumentException>(() =>
                new BoardState(cells: board.Cells.RemoveAt(0)));
        }

        [Fact]
        public void Constructor_MissingCellCoordinate()
        {
            var board = BoardFactory.EmptyBoard();

            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 9; ++col)
                {
                    var wrongCells = board.Cells.Remove(board.Cell(row, col)).Add(BoardFactory.EmptyCell(100, 100));
                    Assert.Throws<ArgumentException>(() => new BoardState(wrongCells));
                }
            }
        }

        public static IEnumerable<object[]> BoxTest_ExpectedCellsPerBox()
        {
            yield return new object[] { 0, new List<Position> { new(0, 0), new(0, 1), new(0, 2), new(1, 0), new(1, 1), new(1, 2), new(2, 0), new(2, 1), new(2, 2) } };
            yield return new object[] { 1, new List<Position> { new(0, 3), new(0, 4), new(0, 5), new(1, 3), new(1, 4), new(1, 5), new(2, 3), new(2, 4), new(2, 5) } };
            yield return new object[] { 2, new List<Position> { new(0, 6), new(0, 7), new(0, 8), new(1, 6), new(1, 7), new(1, 8), new(2, 6), new(2, 7), new(2, 8) } };
            yield return new object[] { 3, new List<Position> { new(3, 0), new(3, 1), new(3, 2), new(4, 0), new(4, 1), new(4, 2), new(5, 0), new(5, 1), new(5, 2) } };
            yield return new object[] { 4, new List<Position> { new(3, 3), new(3, 4), new(3, 5), new(4, 3), new(4, 4), new(4, 5), new(5, 3), new(5, 4), new(5, 5) } };
            yield return new object[] { 5, new List<Position> { new(3, 6), new(3, 7), new(3, 8), new(4, 6), new(4, 7), new(4, 8), new(5, 6), new(5, 7), new(5, 8) } };
            yield return new object[] { 6, new List<Position> { new(6, 0), new(6, 1), new(6, 2), new(7, 0), new(7, 1), new(7, 2), new(8, 0), new(8, 1), new(8, 2) } };
            yield return new object[] { 7, new List<Position> { new(6, 3), new(6, 4), new(6, 5), new(7, 3), new(7, 4), new(7, 5), new(8, 3), new(8, 4), new(8, 5) } };
            yield return new object[] { 8, new List<Position> { new(6, 6), new(6, 7), new(6, 8), new(7, 6), new(7, 7), new(7, 8), new(8, 6), new(8, 7), new(8, 8) } };
        }

        [Theory]
        [MemberData(nameof(BoxTest_ExpectedCellsPerBox))]
        public void Box(int boxId, IEnumerable<Position> expectedPositions)
        {
            var board = BoardFactory.EmptyBoard();
            var boxCells = board.Box(boxId);

            Assert.Equal(expectedPositions, boxCells.Select(c => c.Position));
        }
    }
}
