using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    internal class CollectionPermutator
    {
        public static IEnumerable<int[]> Permutate(int collectionSize, int permutationSize)
        {
            var indices = new int[permutationSize];
            for (int i = 0; i < permutationSize; ++i)
            {
                indices[i] = i;
            }

            yield return indices;
            while (CanIncrement(ref indices, collectionSize))
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
}