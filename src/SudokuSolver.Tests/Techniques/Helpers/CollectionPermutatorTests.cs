using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Tests.Techniques.Helpers;

public class CollectionPermutatorTests
{
    [Theory]
    [InlineData(1, 3, 3)]
    [InlineData(2, 4, 6)]
    [InlineData(2, 9, 36)]
    [InlineData(3, 3, 1)]
    [InlineData(3, 5, 10)]
    [InlineData(3, 9, 84)]
    public void Permutate_CorrectNumberOfPermutationsReturned(int size, int collectionSize, int expectedCount)
    {
        var count = 0;
        foreach (var permutation in CollectionPermutator.Permutate(size, collectionSize))
        {
            count++;
        }
        Assert.Equal(expectedCount, count);
    }

    public static IEnumerable<object[]> PermutationInputs()
    {
        yield return new object[] { 1, 3, new string[] { "0", "1", "2" } };
        yield return new object[] { 2, 3, new string[] { "0,1", "0,2", "1,2" } };
        yield return new object[] { 2, 4, new string[] { "0,1", "0,2", "0,3", "1,2", "1,3", "2,3" } };
        yield return new object[] { 3, 4, new string[] { "0,1,2", "0,1,3", "0,2,3", "1,2,3" } };
        yield return new object[] { 3, 6, new string[] { "0,1,2", "0,1,3", "0,1,4", "0,1,5", "0,2,3", "0,2,4", "0,2,5", "0,3,4", "0,3,5", "0,4,5",
                                                         "1,2,3", "1,2,4", "1,2,5", "1,3,4", "1,3,5", "1,4,5",
                                                         "2,3,4", "2,3,5", "2,4,5",
                                                         "3,4,5" } };
    }

    [Theory]
    [MemberData(nameof(PermutationInputs))]
    public void Permutate_ReturnsExpectedPermutations(int tupleSize, int valueRange, string[] expectedPermutations)
    {
        var permutations = CollectionPermutator.Permutate(tupleSize, valueRange)
            .Select(permutation => string.Join(",", permutation))
            .ToArray();

        Assert.Equal(expectedPermutations, permutations);
    }
}
