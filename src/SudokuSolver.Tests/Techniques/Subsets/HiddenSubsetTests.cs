using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.Subsets;

namespace SudokuSolver.Tests.Techniques.Subsets;

public class HiddenSubsetTests
{
    public static IEnumerable<object[]> FullSets_TestInputs()
    {
        yield return new object[] { 1, RowCellCollector.Instance };
        yield return new object[] { 1, ColumnCellCollector.Instance };
        yield return new object[] { 1, BoxCellCollector.Instance };
        yield return new object[] { 2, RowCellCollector.Instance };
        yield return new object[] { 2, ColumnCellCollector.Instance };
        yield return new object[] { 2, BoxCellCollector.Instance };
        yield return new object[] { 3, RowCellCollector.Instance };
        yield return new object[] { 3, ColumnCellCollector.Instance };
        yield return new object[] { 3, BoxCellCollector.Instance };
        yield return new object[] { 4, RowCellCollector.Instance };
        yield return new object[] { 4, ColumnCellCollector.Instance };
        yield return new object[] { 4, BoxCellCollector.Instance };
    }

    [Theory]
    [MemberData(nameof(FullSets_TestInputs))]
    internal void GetPossibleBoardStateChange(int size, ICellCollector cellCollector)
    {
        var board = BoardFactory.CandidateBoard();
        foreach (var cell in cellCollector.GetCollection(board, 0).Skip(size))
        {
            var candidatesToRemove = Enumerable.Range(1, size)
                .Select(value => new Candidate(cell.Position, value))
                .ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }

        var technique = new HiddenSubset(size, cellCollector);
        var change = technique.GetPossibleBoardStateChange(board);

        foreach (var cell in cellCollector.GetCollection(board, 0).Take(size))
        {
            for (int val = size + 1; val <= 9; ++val)
            {
                Assert.Contains(new Candidate(cell.Position, val), change.Change.CandidatesAffected);
            }
        }
    }

    public static IEnumerable<object[]> IncompleteSets_TestInputs()
    {
        yield return new object[] { 3, RowCellCollector.Instance };
        yield return new object[] { 3, ColumnCellCollector.Instance };
        yield return new object[] { 3, BoxCellCollector.Instance };
        yield return new object[] { 4, RowCellCollector.Instance };
        yield return new object[] { 4, ColumnCellCollector.Instance };
        yield return new object[] { 4, BoxCellCollector.Instance };
    }

    [Theory]
    [MemberData(nameof(IncompleteSets_TestInputs))]
    internal void GetPossibleBoardStateChange_IncompleteSets(int size, ICellCollector cellCollector)
    {
        var board = BoardFactory.CandidateBoard();
        var cells = cellCollector.GetCollection(board, 0).ToList();
        for (int i = 0; i < size; ++i)
        {
            var candidatesToRemove = new List<Candidate>
                {
                    new Candidate(cells[i].Position, i + 1),
                    new Candidate(cells[i].Position, (i + 1) % size + 1),
                };
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }
        foreach (var cell in cells.Skip(size))
        {
            var candidatesToRemove = Enumerable.Range(1, size)
                .Select(value => new Candidate(cell.Position, value))
                .ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(candidatesToRemove));
        }

        var technique = new HiddenSubset(size, cellCollector);
        var change = technique.GetPossibleBoardStateChange(board);

        for (int i = 0; i < size; ++i)
        {
            for (int val = size + 1; val <= 9; ++val)
            {
                Assert.Contains(new Candidate(cells[i].Position, val), change.Change.CandidatesAffected);
            }
        }
    }
}
