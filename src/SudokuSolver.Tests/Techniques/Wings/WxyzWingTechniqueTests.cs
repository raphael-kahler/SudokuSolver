using SudokuSolver.Techniques.Wings;

namespace SudokuSolver.Tests.Techniques.Wings;

public class WxyzWingTechniqueTests
{
    public static IEnumerable<object[]> WxyzWing_TestInputs()
    {
        yield return new object[]
        {
                new List<Cell>
                {
                    Cell.WithCandidates((0, 2), 1, 2, 3, 4),
                    Cell.WithCandidates((0, 4), 2, 4),
                    Cell.WithCandidates((0, 6), 3, 4),
                    Cell.WithCandidates((1, 1), 1, 4),
                },
                new HashSet<Candidate> { new((0, 0), 4), new((0, 1), 4) }
        };
        yield return new object[]
        {
                new List<Cell>
                {
                    Cell.WithCandidates((8, 1), 4, 5, 8),
                    Cell.WithCandidates((8, 0), 4, 5),
                    Cell.WithCandidates((5, 1), 7, 8),
                    Cell.WithCandidates((2, 1), 4, 7),
                },
                new HashSet<Candidate> { new((6, 1), 4), new((7, 1), 4) }
        };
    }

    [Theory]
    [MemberData(nameof(WxyzWing_TestInputs))]
    public void GetPossibleBoardStateChange_WxyzWing(List<Cell> wingCells, HashSet<Candidate> affectedCandidates)
    {
        var board = BoardFactory.CandidateBoard();
        foreach (var cell in wingCells)
        {
            var removals = Enumerable.Range(1, 9).Except(cell.Candidates).Select(value => new Candidate(cell.Position, value)).ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(removals));
        }

        var technique = new WxyzWingTechnique();
        var change = technique.GetPossibleBoardStateChange(board);

        change.Change.CandidatesAffected.Should().BeEquivalentTo(affectedCandidates);
    }
}
