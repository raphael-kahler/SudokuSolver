using System.Linq;
using SudokuSolver.Techniques;
using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests.Techniques
{
    public class NakedSubsetTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GetPossibleBoardStateChange_Row(int size)
        {
            var board = BoardFactory.CandidateBoard();
            for (int i = 0; i < size; ++i)
            {
                var candidatesToRemove = Enumerable.Range(size + 1, 9 - size)
                    .Select(v => new Candidate(new Position(0, i), v))
                    .ToList();
                board = board.ApplyChange(new BoardStateChangeCandidateRemoval(candidatesToRemove));
            }

            var technique = new NakedSubset(size, RowCellCollector.Instance);
            var change = technique.GetPossibleBoardStateChange(board);

            Assert.IsType<BoardStateChangeCandidateRemoval>(change);
            var removalChange = change as BoardStateChangeCandidateRemoval;
            for (int col = size; col < 9; ++col)
            {
                for (int val = 1; val <= size; ++val)
                {
                    Assert.Contains(new Candidate(new Position(0, col), val), removalChange.CandidatesToRemove);
                }
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GetPossibleBoardStateChange_Column(int size)
        {
            var board = BoardFactory.CandidateBoard();
            for (int i = 0; i < size; ++i)
            {
                var candidatesToRemove = Enumerable.Range(size + 1, 9 - size)
                    .Select(v => new Candidate(new Position(i, 0), v))
                    .ToList();
                board = board.ApplyChange(new BoardStateChangeCandidateRemoval(candidatesToRemove));
            }

            var technique = new NakedSubset(size, ColumnCellCollector.Instance);
            var change = technique.GetPossibleBoardStateChange(board);

            Assert.IsType<BoardStateChangeCandidateRemoval>(change);
            var removalChange = change as BoardStateChangeCandidateRemoval;
            for (int row = size; row < 9; ++row)
            {
                for (int val = 1; val <= size; ++val)
                {
                    Assert.Contains(new Candidate(new Position(row, 0), val), removalChange.CandidatesToRemove);
                }
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GetPossibleBoardStateChange_Box(int size)
        {
            var board = BoardFactory.CandidateBoard();
            for (int i = 0; i < size; ++i)
            {
                Position position = board.Box(0).Skip(i).First().Position;
                var candidatesToRemove = Enumerable.Range(size + 1, 9 - size)
                    .Select(v => new Candidate(position, v))
                    .ToList();
                board = board.ApplyChange(new BoardStateChangeCandidateRemoval(candidatesToRemove));
            }

            var technique = new NakedSubset(size, BoxCellCollector.Instance);
            var change = technique.GetPossibleBoardStateChange(board);

            Assert.IsType<BoardStateChangeCandidateRemoval>(change);
            var removalChange = change as BoardStateChangeCandidateRemoval;
            foreach (var cell in board.Box(0).Skip(size))
            {
                for (int val = 1; val <= size; ++val)
                {
                    Assert.Contains(new Candidate(cell.Position, val), removalChange.CandidatesToRemove);
                }
            }
        }
    }
}