namespace SudokuSolver.Tests;

public class BoardStateChangeTests
{
    [Fact]
    public void SetNumber_SetsNumberCorrectly()
    {
        var board = BoardFactory.CandidateBoard();
        var change = BoardStateChange.SetCell(new Position(1, 2), value: 5);
        var newBoard = change.ApplyTo(board);

        Assert.False(board.Cell(1, 2).Value.HasValue);
        Assert.True(newBoard.Cell(1, 2).Value.HasValue);
        Assert.Equal(5, newBoard.Cell(1, 2).Value);
    }

    [Fact]
    public void RemoveCandidates_RemovesCorrectCandidateNumbers()
    {
        var board = BoardFactory.CandidateBoard();
        var candidateRemovals = new List<Candidate> { new((1, 2), 7), new((8, 8), 2) };
        var change = BoardStateChange.RemoveCandidates(candidateRemovals);
        var newBoard = change.ApplyTo(board);

        foreach (var removal in candidateRemovals)
        {
            Assert.Contains(removal.CandidateValue, board.Cell(removal.Position).Candidates);
            Assert.DoesNotContain(removal.CandidateValue, newBoard.Cell(removal.Position).Candidates);
        }
    }

    [Fact]
    public void RemoveCandidates_MultipleRempovalsOnSameCell()
    {
        var board = BoardFactory.CandidateBoard();
        var candidateRemovals = new List<Candidate> { new((1, 1), 7), new((1, 1), 2) };
        var change = BoardStateChange.RemoveCandidates(candidateRemovals);
        var newBoard = change.ApplyTo(board);

        foreach (var removal in candidateRemovals)
        {
            Assert.Contains(removal.CandidateValue, board.Cell(removal.Position).Candidates);
            Assert.DoesNotContain(removal.CandidateValue, newBoard.Cell(removal.Position).Candidates);
        }
    }

    [Fact]
    public void NoChange_KeepsCellsIdentical()
    {
        var board = BoardFactory.CandidateBoard();
        var newBoard = board.ApplyChange(BoardStateNoChange.Instance);

        Assert.Equal(board.Cells, newBoard.Cells);
    }
}
