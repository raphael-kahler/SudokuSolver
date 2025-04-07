using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.Subsets;

namespace SudokuSolver.Tests.Techniques.Subsets;

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
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }

        var technique = new NakedSubset(size, RowCellCollector.Instance);
        var change = technique.GetPossibleBoardStateChange(board);

        for (int col = size; col < 9; ++col)
        {
            for (int val = 1; val <= size; ++val)
            {
                Assert.Contains(new Candidate(new Position(0, col), val), change.Change.CandidatesAffected);
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
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }

        var technique = new NakedSubset(size, ColumnCellCollector.Instance);
        var change = technique.GetPossibleBoardStateChange(board);

        for (int row = size; row < 9; ++row)
        {
            for (int val = 1; val <= size; ++val)
            {
                Assert.Contains(new Candidate(new Position(row, 0), val), change.Change.CandidatesAffected);
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
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }

        var technique = new NakedSubset(size, BoxCellCollector.Instance);
        var change = technique.GetPossibleBoardStateChange(board);

        foreach (var cell in board.Box(0).Skip(size))
        {
            for (int val = 1; val <= size; ++val)
            {
                Assert.Contains(new Candidate(cell.Position, val), change.Change.CandidatesAffected);
            }
        }
    }

    [Fact]
    public void GetPossibleBoardStateChange_Triple_Row_EachCellOnlyContainsASubsetOfCandidates()
    {
        var technique = Technique.Subsets.NakedTripleRow();
        var board = BoardFactory.CandidateBoard()
            .ApplyChange(BoardStateChange.SetCell(Cell.WithCandidates((0, 0), 1, 2)))
            .ApplyChange(BoardStateChange.SetCell(Cell.WithCandidates((0, 1), 2, 3)))
            .ApplyChange(BoardStateChange.SetCell(Cell.WithCandidates((0, 2), 1, 3)));

        var change = technique.GetPossibleBoardStateChange(board);

        Assert.True(change.Change.HasEffect);
        Assert.Equal(18, change.Change.CandidatesAffected.Count);
        for (int col = 3; col < 9; ++col)
        {
            for (int value = 1; value <= 3; ++value)
            {
                Assert.Contains(new Candidate((0, col), value), change.Change.CandidatesAffected);
            }
        }
    }

    public static IEnumerable<object[]> GetPossibleBoardStateChange_Pair_Row_NoChangeCombinations_Inputs()
    {
        yield return new object[] { new List<Cell> {
                Cell.WithCandidates((0, 2), 4, 9),
                Cell.WithValue((0, 6), 3)
            } };
        yield return new object[] { new List<Cell> {
                Cell.WithCandidates((0, 2), 4, 9),
                Cell.WithCandidates((0, 6), 4, 6)
            } };
        yield return new object[] { new List<Cell> {
                Cell.WithCandidates((0, 2), 4, 9),
                Cell.WithCandidates((0, 6), 4, 6)
            } };
    }

    [Theory]
    [MemberData(nameof(GetPossibleBoardStateChange_Pair_Row_NoChangeCombinations_Inputs))]
    public void GetPossibleBoardStateChange_Pair_Row_NoChangeCombinations(IList<Cell> cellsToSet)
    {
        var technique = Technique.Subsets.NakedPairRow();
        var board = BoardFactory.CandidateBoard();
        foreach (var cell in cellsToSet)
        {
            board = board.ApplyChange(BoardStateChange.SetCell(cell));
        }

        var change = technique.GetPossibleBoardStateChange(board);

        Assert.False(change.Change.HasEffect);
    }
}
