using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Techniques;
using Xunit;

namespace SudokuSolver.Tests.Techniques
{
    public class FishTechniqueTests
    {
        public static IEnumerable<object[]> FishTechnique_BoardStates()
        {
            // inputs are: (bool) isRowFish, (set) fish positions, (set) positions affected, (set) positions not affected

            // row fish x-wing pattern
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(2, 4), new(2, 8), new(4, 4), new(4, 8) },
                new HashSet<Position> { new(0, 4), new(3, 8), new(5, 8), new(8, 8) },
                new HashSet<Position> { new(0, 2), new(0, 5), new(1, 2), new(1, 6), new(1, 7), new(8, 0), new(8, 7) }
            };
            // row fish sashimi x-wing pattern
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(2, 4), new(2, 8), new(4, 4), new(4, 7) },
                new HashSet<Position> { new(1, 7), new(3, 8), new(5, 8) },
                new HashSet<Position> { new(0, 2), new(0, 4), new(0, 5), new(1, 2), new(1, 6), new(8, 0), new(8, 7), new(8, 8) }
            };
            // row fish sashimi x-wing pattern with two possible fins
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(4, 3), new(4, 8), new(6, 3), new(6, 6) },
                new HashSet<Position> { new(3, 6), new(5, 6), new(7, 8), new(8, 8) },
                new HashSet<Position> { new(7, 2), new(7, 4), new(7, 5), new(8, 2), new(8, 5), new(8, 7) }
            };
            // row fish sashimi x-wing pattern (same as above but with extra fin, now only one set of affected cells)
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(4, 3), new(4, 8), new(6, 3), new(6, 6), new(6, 7) },
                new HashSet<Position> { new(7, 8), new(8, 8) },
                new HashSet<Position> { new(3, 6), new(5, 6), new(7, 2), new(7, 4), new(7, 5), new(8, 2), new(8, 5), new(8, 7) }
            };
            // row fish sashimi x-wing pattern (same as above but with extra fin on top of regular position, now only one set of affected cells)
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(4, 3), new(4, 8), new(6, 3), new(6, 6), new(6, 8) },
                new HashSet<Position> { new(7, 8), new(8, 8) },
                new HashSet<Position> { new(3, 6), new(5, 6), new(7, 2), new(7, 4), new(7, 5), new(8, 2), new(8, 5), new(8, 7) }
            };
            // column fish x-wing pattern
            yield return new object[]
            {
                FishTechnique.TwoColumn(),
                new HashSet<Position> { new(1, 1), new(1, 7), new(7, 1), new(7, 7) },
                new HashSet<Position> { new(1, 4), new(1, 6), new(7, 0), new(7, 4) },
                new HashSet<Position> { new(2, 0), new(2, 6), new(4, 0), new(4, 4), new(4, 6) }
            };
            // column fish sashimi x-wing pattern
            yield return new object[]
            {
                FishTechnique.TwoColumn(),
                new HashSet<Position> { new(1, 1), new(2, 7), new(7, 1), new(7, 7) },
                new HashSet<Position> { new(1, 6), new(2, 0) },
                new HashSet<Position> { new(1, 4), new(2, 6), new(4, 0), new(4, 4), new(4, 6), new(7, 0), new(7, 4) }
            };
            // row fish x-wing with two fins
            yield return new object[]
            {
                FishTechnique.TwoRow(),
                new HashSet<Position> { new(4, 1), new(4, 4), new(7, 1), new(7, 3), new(7, 4), new(7, 5) },
                new HashSet<Position> { new(6, 4), new(8, 4) },
                new HashSet<Position> { new(1, 1), new(5, 4), new(6, 3) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeRow(),
                new HashSet<Position> { new(1, 1), new(1, 7), new(4, 1), new(4, 4), new(7, 4), new(7, 6) },
                new HashSet<Position> { new(0, 6), new(6, 7), new(8, 7) },
                new HashSet<Position> { new(0, 4), new(0, 8), new(3, 1), new(3, 4), new(3, 6), new(3, 7), new(3, 8) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeRow(),
                new HashSet<Position> { new(1, 1), new(1, 7), new(4, 1), new(4, 4), new(4, 7), new(7, 4), new(7, 6) },
                new HashSet<Position> { new(6, 7), new(8, 7) },
                new HashSet<Position> { new(0, 4), new(0, 6), new(0, 8), new(3, 1), new(3, 4), new(3, 6), new(3, 7), new(3, 8) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeRow(),
                new HashSet<Position> { new(2, 0), new(2, 2), new(4, 0), new(4, 8), new(6, 0), new(6, 2), new(6, 6), new(6, 8) },
                new HashSet<Position> { new(7, 8), new(8, 8) },
                new HashSet<Position> { new(7, 0), new(7, 2), new(7, 5), new(7, 6), new(7, 7), new(8, 0), new(8, 2), new(8, 5), new(8, 6), new(8, 7) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeRow(),
                new HashSet<Position> { new(2, 0), new(2, 2), new(4, 0), new(4, 8), new(6, 0), new(6, 2), new(6, 6), new(6, 8) },
                new HashSet<Position> { new(7, 8) },
                new HashSet<Position> { new(3, 0), new(3, 2), new(3, 3), new(3, 6), new(3, 8), new(7, 0), new(7, 1), new(7, 2), new(7, 6) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeColumn(),
                new HashSet<Position> { new(1, 0), new(1, 4), new(2, 0), new(2, 4), new(2, 8), new(3, 0), new(4, 0), new(4, 4), new(4, 8) },
                new HashSet<Position> { new(4, 1) },
                new HashSet<Position> { new(0, 6), new(1, 6), new(2, 1), new(2, 5), new(2, 7), new(3, 1), new(3, 5), new(3, 6), new(3, 7), new(4, 6), new(4, 7), new(5, 5), new(5, 6), new(7, 6), new(7, 7) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeColumn(),
                new HashSet<Position> { new(1, 0), new(1, 4), new(2, 0), new(2, 4), new(2, 8), new(3, 0), new(4, 0), new(4, 4), new(4, 8), new(5, 0) },
                new HashSet<Position> { new(4, 1) },
                new HashSet<Position> { new(0, 6), new(1, 6), new(2, 1), new(2, 5), new(2, 7), new(3, 1), new(3, 5), new(3, 6), new(3, 7), new(4, 6), new(4, 7), new(5, 5), new(5, 6), new(7, 6), new(7, 7) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeColumn(),
                new HashSet<Position> { new(1, 0), new(1, 4), new(2, 0), new(2, 4), new(2, 8), new(4, 0), new(4, 4), new(4, 8) },
                new HashSet<Position> { new(1, 6), new(2, 1), new(2, 5), new(2, 7), new(4, 1), new(4, 6), new(4, 7) },
                new HashSet<Position> { new(0, 6), new(3, 1), new(3, 5), new(3, 6), new(3, 7), new(5, 5), new(5, 6), new(7, 6), new(7, 7) }
            };
            yield return new object[]
            {
                FishTechnique.ThreeColumn(),
                new HashSet<Position> { new(2, 1), new(2, 7), new(2, 8), new(3, 1), new(4, 1), new(4, 7), new(4, 8), new(8, 7), new(8, 8) },
                new HashSet<Position> { new(4, 2) },
                new HashSet<Position> { new(0, 2), new(0, 3), new(0, 5), new(0, 6), new(1, 2), new(1, 6), new(2, 1), new(2, 5), new(2, 7), new(2, 8), new(3, 5), new(3, 6), new(4, 6), new(5 ,3), new(5, 5), new(5, 6) }
            };
            yield return new object[]
            {
                FishTechnique.FourRow(),
                new HashSet<Position> { new(1, 1), new(1, 2), new(1, 7), new(2, 2), new(2, 7), new(5, 1), new(5, 5), new(5, 7), new(6, 1), new(6, 4), new(6, 7) },
                new HashSet<Position> { new(3, 4), new(4, 4), new(7, 5), new(8, 5) },
                new HashSet<Position> { }
            };
            yield return new object[]
            {
                FishTechnique.FourRow(),
                new HashSet<Position> { new(1, 1), new(1, 5), new(2, 5), new(2, 7), new(5, 3), new(5, 7), new(6, 1), new(6, 3) },
                new HashSet<Position> { new(0, 1), new(3, 5), new(7, 3), new(8, 7) },
                new HashSet<Position> { new(4, 4)}
            };
        }

        [Theory]
        [MemberData(nameof(FishTechnique_BoardStates))]
        public void GetPossibleBoardStateChange_FishTechnique(
            FishTechnique fishTechnique,
            HashSet<Position> fishPositions,
            HashSet<Position> positionsAffected,
            HashSet<Position> positionsNotAffected)
        {
            // set up board
            int candidateValue = 7;
            var board = BoardFactory.EmptyBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !positionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals, NotFound.Instance, NoChangeDescription.Instance));

            // use fish technique
            var boardChange = fishTechnique.GetPossibleBoardStateChange(board);

            // validate
            Assert.IsType<BoardStateChangeCandidateRemoval>(boardChange);
            var candidateRemovalChange = boardChange as BoardStateChangeCandidateRemoval;
            Assert.Equal(positionsAffected, candidateRemovalChange.CandidatesToRemove.Select(r => r.Position).ToHashSet());
        }
    }
}