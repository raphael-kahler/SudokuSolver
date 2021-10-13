using SudokuSolver.Techniques.Coloring;

namespace SudokuSolver.Tests.Techniques.Coloring;

public class SimpleColorsTechniqueTests
{
    [Fact]
    public void GetPossibleBoardStateChange()
    {
        var board = BoardFactory.EmptyBoard();
        foreach (var position in new List<Position> { (0, 3), (0, 5), (0, 7), (1, 5), (1, 6), (1, 8), (4, 6), (4, 7), (6, 2), (6, 3), (6, 5), (7, 2), (7, 3), (7, 8), (8, 5), (8, 6) })
        {
            board = board.ApplyChange(BoardStateChange.SetCell(Cell.WithCandidates(position, 3)));
        }

        var technique = new SimpleColorsTechnique();

        var change = technique.GetPossibleBoardStateChange(board);

        Assert.Contains(new Candidate((1, 5), 3), change.Change.CandidatesAffected);
        Assert.Equal(2, change.Change.CandidatesCausingChange.Count);
        change.Change.CandidatesCausingChange[0].Select(c => c.Position).Should().BeEquivalentTo(new List<Position> { (7, 8), (8, 5) });
        change.Change.CandidatesCausingChange[1].Select(c => c.Position).Should().BeEquivalentTo(new List<Position> { (1, 8), (8, 6) });
    }
}
