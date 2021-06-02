using System.Collections.Immutable;
using Xunit;

namespace SudokuSolver.Tests
{
    public class BoardStateChangeTests
    {
        [Fact]
        public void SetNumber_SetsNumberCorrectly()
        {
            var board = BoardFactory.CandidateBoard();
            var change = new BoardStateChangeSetNumber(new Position(1, 2), value: 5);
            var newBoard = change.ApplyTo(board);

            Assert.False(board.Cell(1, 2).Value.HasValue);
            Assert.True(newBoard.Cell(1, 2).Value.HasValue);
            Assert.Equal(5, newBoard.Cell(1, 2).Value);
        }

        [Fact]
        public void SetNumber_SetsLastChange()
        {
            var board = BoardFactory.CandidateBoard();
            var change = new BoardStateChangeSetNumber(new Position(1, 2), value: 5);
            var newBoard = change.ApplyTo(board);

            Assert.NotEqual(change, board.LastChange);
            Assert.Equal(change, newBoard.LastChange);
        }

        [Fact]
        public void RemoveCandidate_SetsLastChange()
        {
            var board = BoardFactory.CandidateBoard();
            var canidateRemovals = ImmutableList<Candidate>.Empty
                .Add(new Candidate(new Position(1, 2), CandidateValue: 7));
            var change = new BoardStateChangeCandidateRemoval(canidateRemovals);
            var newBoard = change.ApplyTo(board);

            Assert.NotEqual(change, board.LastChange);
            Assert.Equal(change, newBoard.LastChange);
        }

        [Fact]
        public void RemoveCandidate_RemovesCorrectCandidateNumbers()
        {
            var board = BoardFactory.CandidateBoard();
            var canidateRemovals = ImmutableList<Candidate>.Empty
                .Add(new Candidate(new Position(1, 2), CandidateValue: 7))
                .Add(new Candidate(new Position(8, 8), CandidateValue: 2));
            var change = new BoardStateChangeCandidateRemoval(canidateRemovals);
            var newBoard = change.ApplyTo(board);

            foreach (var removal in canidateRemovals)
            {
                Assert.Contains(removal.CandidateValue, board.Cell(removal.Position).Candidates);
                Assert.DoesNotContain(removal.CandidateValue, newBoard.Cell(removal.Position).Candidates);
            }
        }

        [Fact]
        public void RemoveCandidate_MultipleRempovalsOnSameCell()
        {
            var board = BoardFactory.CandidateBoard();
            var canidateRemovals = ImmutableList<Candidate>.Empty
                .Add(new Candidate(new Position(1, 1), CandidateValue: 7))
                .Add(new Candidate(new Position(1, 1), CandidateValue: 2));
            var change = new BoardStateChangeCandidateRemoval(canidateRemovals);
            var newBoard = change.ApplyTo(board);

            foreach (var removal in canidateRemovals)
            {
                Assert.Contains(removal.CandidateValue, board.Cell(removal.Position).Candidates);
                Assert.DoesNotContain(removal.CandidateValue, newBoard.Cell(removal.Position).Candidates);
            }
        }

        [Fact]
        public void NoChange_KeepsCellsIdentical()
        {
            var board = BoardFactory.CandidateBoard();
            var newBoard = board.ApplyChange(new BoardStateNoChange());

            Assert.Equal(board.Cells, newBoard.Cells);
        }


        [Fact]
        public void NoChange_SetsLastChange()
        {
            var board = BoardFactory.CandidateBoard()
                .ApplyChange(new BoardStateChangeSetNumber(new Position(1, 1), 4));
            var change = new BoardStateNoChange();
            var newBoard = board.ApplyChange(change);

            Assert.NotEqual(change, board.LastChange);
            Assert.Equal(change, newBoard.LastChange);
        }
    }
}
