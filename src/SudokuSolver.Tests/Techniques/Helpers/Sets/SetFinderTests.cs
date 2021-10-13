using SudokuSolver.Techniques.Helpers.Sets;

namespace SudokuSolver.Tests.Techniques.Helpers.Sets;

public class SetFinderTests
{
    public static IEnumerable<object[]> AlmostLockedSet_TestInputs()
    {
        yield return new object[]
        {
                2,
                new List<Cell>
                {
                    Cell.WithCandidates((0, 0), 1, 2, 3, 4, 5),
                    Cell.WithCandidates((0, 1), 1, 2, 3),
                    Cell.WithCandidates((0, 2), 2, 3),
                    Cell.WithCandidates((0, 3), 3, 4, 5),
                    Cell.WithCandidates((0, 4), 1, 2),
                },
                new List<IList<int>>
                {
                    new int[] { 1, 2 },
                    new int[] { 1, 4 },
                    new int[] { 2, 4 },
                }
        };
        yield return new object[]
        {
                3,
                new List<Cell>
                {
                    Cell.WithCandidates((0, 0), 1, 2, 3, 4, 5, 6),
                    Cell.WithCandidates((0, 1), 1, 2, 3, 4),
                    Cell.WithCandidates((0, 2), 1, 2, 3),
                    Cell.WithCandidates((0, 3), 2, 3, 4),
                    Cell.WithCandidates((0, 4), 4, 5, 6),
                    Cell.WithCandidates((0, 5), 1, 2),
                    Cell.WithCandidates((0, 6), 3, 4),
                },
                new List<IList<int>>
                {
                    new int[] { 1, 2, 3 },
                    new int[] { 1, 2, 5 },
                    new int[] { 1, 2, 6 },
                    new int[] { 1, 3, 5 },
                    new int[] { 1, 3, 6 },
                    new int[] { 1, 5, 6 },
                    new int[] { 2, 3, 5 },
                    new int[] { 2, 3, 6 },
                    new int[] { 2, 5, 6 },
                    new int[] { 3, 5, 6 },
                }
        };
    }

    [Theory]
    [MemberData(nameof(AlmostLockedSet_TestInputs))]
    public void FindAlmostLockedSets_FindsCorrectSets(int size, IList<Cell> cells, IList<IList<int>> expectedSets)
    {
        var almostLockedSets = SetFinder.FindAlmostLockedSets(cells, size).ToList();
        Assert.Equal(expectedSets.Count, almostLockedSets.Count);
        foreach (var expectedSet in expectedSets)
        {
            var set = new AlmostLockedSet(expectedSet.Select(idx => cells[idx]).ToList());
            // var set = expectedSet.Select(idx => cells[idx]);
            Assert.Contains(set, almostLockedSets);
        }
    }
}
