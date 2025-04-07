namespace SudokuSolver.Techniques.Helpers;

internal class CollectionPermutator
{
    /// <summary>
    /// Create all possible permuations of a tuple of numbers. The tuples are returned in ascending sort order.
    /// </summary>
    /// <remarks>
    /// Permutating <paramref name="tupleSize"/> = 2 and <paramref name="valueRange"/> = 4 will return the tuples
    /// [ "0,1", "0,2", "0,3", "1,2", "1,3", "2,3" ].
    /// </remarks>
    /// <param name="tupleSize">How many numbers should be in tuple that is permutated.</param>
    /// <param name="valueRange">The range of values each number in the tuple can take on. The tuple numbers will go from 0 to (<paramref name="valueRange"/> - 1).</param>
    /// <returns>The collection of all possible permutations.</returns>
    public static IEnumerable<int[]> Permutate(int tupleSize, int valueRange)
    {
        var indices = new int[tupleSize];
        for (int i = 0; i < tupleSize; ++i)
        {
            indices[i] = i;
        }

        yield return indices;
        while (CanIncrement(ref indices, valueRange))
        {
            yield return indices;
        }
    }

    private static bool CanIncrement(ref int[] indices, int collectionSize)
    {
        for (int i = indices.Length - 1; i >= 0; --i)
        {
            var val = ++indices[i];
            if (!BiggerThanMax(val, i, indices.Length, collectionSize))
            {
                for (int j = i + 1; j < indices.Length; ++j)
                {
                    indices[j] = indices[j - 1] + 1;
                }
                return true;
            }
        }
        return false;
    }

    private static bool BiggerThanMax(int value, int index, int indexCount, int collectionSize) => value > collectionSize - indexCount + index;
}
