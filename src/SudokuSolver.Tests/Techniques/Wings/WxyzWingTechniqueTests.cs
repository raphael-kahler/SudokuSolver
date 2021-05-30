using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Techniques;
using SudokuSolver.Techniques.Wings;
using Xunit;

namespace SudokuSolver.Tests.Techniques.Wings
{
    public class WxyzWingTechniqueTests
    {
        public static IEnumerable<object[]> WxyzWing_TestInputs()
        {
            yield return new object[]
            {
                new List<Cell>
                {
                    new Cell((0, 2), 1, 2, 3, 4),
                    new Cell((0, 4), 2, 4),
                    new Cell((0, 6), 3, 4),
                    new Cell((1, 1), 1, 4),
                },
                new HashSet<Candidate> { new((0, 0), 4), new((0, 1), 4) }
            };
            yield return new object[]
            {
                new List<Cell>
                {
                    new Cell((8, 1), 4, 5, 8),
                    new Cell((8, 0), 4, 5),
                    new Cell((5, 1), 7, 8),
                    new Cell((2, 1), 4, 7),
                },
                new HashSet<Candidate> { new((6, 1), 4), new((7, 1), 4) }
            };
        }

        [Theory]
        [MemberData(nameof(WxyzWing_TestInputs))]
        public void GetPossibleBoardStateChange_WxyzWing(List<Cell> wingCells, HashSet<Candidate> affectedCandidates)
        {
            var board = BoardFactory.EmptyBoard();
            foreach (var cell in wingCells)
            {
                var removals = Enumerable.Range(1, 9).Except(cell.Candidates).Select(value => new Candidate(cell.Position, value)).ToList();
                board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals, NotFound.Instance, NoChangeDescription.Instance));
            }

            var technique = new WxyzWingTechnique();
            var change = technique.GetPossibleBoardStateChange(board);

            if (affectedCandidates.Any())
            {
                Assert.IsType<BoardStateChangeCandidateRemoval>(change);
                var removalChange = change as BoardStateChangeCandidateRemoval;
                Assert.Equal(affectedCandidates, removalChange.CandidatesToRemove.ToHashSet());
            }
            else
            {
                Assert.IsType<BoardStateNoChange>(change);
            }
        }
    }
}