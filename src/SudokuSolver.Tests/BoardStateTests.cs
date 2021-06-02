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
            var board = BoardFactory.CandidateBoard();

            Assert.Throws<ArgumentException>(() =>
                new BoardState(cells: board.Cells.RemoveAt(0)));
        }

        [Fact]
        public void Constructor_MissingCellCoordinate()
        {
            var board = BoardFactory.CandidateBoard();

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
            yield return new object[] { 0, new List<Position> { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2) } };
            yield return new object[] { 1, new List<Position> { (0, 3), (0, 4), (0, 5), (1, 3), (1, 4), (1, 5), (2, 3), (2, 4), (2, 5) } };
            yield return new object[] { 2, new List<Position> { (0, 6), (0, 7), (0, 8), (1, 6), (1, 7), (1, 8), (2, 6), (2, 7), (2, 8) } };
            yield return new object[] { 3, new List<Position> { (3, 0), (3, 1), (3, 2), (4, 0), (4, 1), (4, 2), (5, 0), (5, 1), (5, 2) } };
            yield return new object[] { 4, new List<Position> { (3, 3), (3, 4), (3, 5), (4, 3), (4, 4), (4, 5), (5, 3), (5, 4), (5, 5) } };
            yield return new object[] { 5, new List<Position> { (3, 6), (3, 7), (3, 8), (4, 6), (4, 7), (4, 8), (5, 6), (5, 7), (5, 8) } };
            yield return new object[] { 6, new List<Position> { (6, 0), (6, 1), (6, 2), (7, 0), (7, 1), (7, 2), (8, 0), (8, 1), (8, 2) } };
            yield return new object[] { 7, new List<Position> { (6, 3), (6, 4), (6, 5), (7, 3), (7, 4), (7, 5), (8, 3), (8, 4), (8, 5) } };
            yield return new object[] { 8, new List<Position> { (6, 6), (6, 7), (6, 8), (7, 6), (7, 7), (7, 8), (8, 6), (8, 7), (8, 8) } };
        }

        [Theory]
        [MemberData(nameof(BoxTest_ExpectedCellsPerBox))]
        public void Box(int boxId, IEnumerable<Position> expectedPositions)
        {
            var board = BoardFactory.CandidateBoard();
            var boxCells = board.Box(boxId);

            Assert.Equal(expectedPositions, boxCells.Select(c => c.Position));
        }

        [Fact]
        public void Cell_Equality()
        {
            Assert.Equal(
                new Cell((1, 2), 1, 4, 7),
                new Cell((1, 2), 1, 4, 7)
            );
        }

        [Fact]
        public void Cell_Equality_Null()
        {
            Assert.NotEqual(new Cell((1, 2), 1, 4, 7), null);
            Assert.NotEqual(null, new Cell((1, 2), 1, 4, 7));
        }


        [Fact]
        public void Cell_Equals_Null()
        {
            Assert.False(new Cell((1, 2), 1, 4, 7).Equals(null));
        }
    }
}
