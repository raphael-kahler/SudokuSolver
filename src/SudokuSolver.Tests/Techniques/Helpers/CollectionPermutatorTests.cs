using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests.Techniques.Helpers
{
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
            foreach (var permutation in CollectionPermutator.Permutate(collectionSize, size))
            {
                count++;
            }
            Assert.Equal(expectedCount, count);
        }
    }
}