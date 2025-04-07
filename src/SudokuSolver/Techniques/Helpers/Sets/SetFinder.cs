namespace SudokuSolver.Techniques.Helpers.Sets;

internal static class SetFinder
{
    public static IEnumerable<AlmostLockedSet> FindAlmostLockedSets(IEnumerable<Cell> cellCollection, int size)
    {
        var cellList = cellCollection.Where(c => c.Candidates.Any()).ToList();
        if (cellList.Count < size)
        {
            yield break;
        }

        foreach (var permutation in CollectionPermutator.Permutate(size, cellList.Count))
        {
            var cells = permutation.Select(idx => cellList[idx]);
            var candidateCount = cells.SelectMany(c => c.Candidates).Distinct().Count();
            if (candidateCount == size + 1)
            {
                yield return new AlmostLockedSet(cells.ToList());
            }
        }
    }
}
