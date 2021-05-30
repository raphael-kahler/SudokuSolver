using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Techniques;
using SudokuSolver.Techniques.Wings;
using Xunit;

namespace SudokuSolver.Tests.Techniques.Wings
{
    public class XyzWingTechniqueTests
    {
        public static IEnumerable<object[]> XyzWing_TestInputs()
        {
            yield return new object[]
            {
                new Position(2, 2), new Position(2, 1), new Position(6, 2),
                new List<Position> { (0, 2), (1, 2) }
            };
            yield return new object[]
            {
                new Position(0, 2), new Position(2, 0), new Position(0, 6),
                new List<Position> { (0, 0), (0, 1) }
            };
            yield return new object[]
            {
                new Position(3, 5), new Position(3, 2), new Position(6, 5),
                new List<Position> { }
            };
        }

        [Theory]
        [MemberData(nameof(XyzWing_TestInputs))]
        public void GetPossibleBoardStateChange_XyzWing(Position xyzPosition, Position xzPosition, Position yzPosition, List<Position> affectedPositions)
        {
            // create board where xyz has candidates 123, xz has 12, and yz has 13
            var board = BoardFactory.EmptyBoard();
            var removals = Enumerable.Range(4, 6).Select(value => new Candidate(xyzPosition, value))
                .Concat(Enumerable.Range(3, 7).Select(value => new Candidate(xzPosition, value)))
                .Concat(Enumerable.Range(4, 7).Append(2).Select(value => new Candidate(yzPosition, value)))
                .ToList();
            board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals, NotFound.Instance, NoChangeDescription.Instance));

            var technique = new XyzWingTechnique();
            var change = technique.GetPossibleBoardStateChange(board);

            if (affectedPositions.Any())
            {
                Assert.IsType<BoardStateChangeCandidateRemoval>(change);
                var removalChange = change as BoardStateChangeCandidateRemoval;
                Assert.Equal(affectedPositions, removalChange.CandidatesToRemove.Select(c => c.Position).OrderBy(p => p.Row).ThenBy(p => p.Col));
            }
            else
            {
                Assert.IsType<BoardStateNoChange>(change);
            }
        }
    }
}