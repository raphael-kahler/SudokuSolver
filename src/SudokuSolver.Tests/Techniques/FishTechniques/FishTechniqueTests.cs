using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Techniques.FishTechniques;
using Xunit;

namespace SudokuSolver.Tests.Techniques.FishTechniques
{
    public class FishTechniqueTests
    {
        public static IEnumerable<object[]> FishTechnique_BoardStates()
        {
            // inputs are: (bool) isRowFish, (set) fish positions, (set) positions affected, (set) positions not affected

            // row fish x-wing pattern
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (2, 4), (2, 8), (4, 4), (4, 8) },
                new HashSet<Position> { (0, 4), (3, 8), (5, 8), (8, 8) },
                new HashSet<Position> { (0, 2), (0, 5), (1, 2), (1, 6), (1, 7), (8, 0), (8, 7) }
            };
            // row fish sashimi x-wing pattern
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (2, 4), (2, 8), (4, 4), (4, 7) },
                new HashSet<Position> { (1, 7), (3, 8), (5, 8) },
                new HashSet<Position> { (0, 2), (0, 4), (0, 5), (1, 2), (1, 6), (8, 0), (8, 7), (8, 8) }
            };
            // row fish sashimi x-wing pattern with two possible fins
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (4, 3), (4, 8), (6, 3), (6, 6) },
                new HashSet<Position> { (3, 6), (5, 6), (7, 8), (8, 8) },
                new HashSet<Position> { (7, 2), (7, 4), (7, 5), (8, 2), (8, 5), (8, 7) }
            };
            // row fish sashimi x-wing pattern (same as above but with extra fin, now only one set of affected cells)
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (4, 3), (4, 8), (6, 3), (6, 6), (6, 7) },
                new HashSet<Position> { (7, 8), (8, 8) },
                new HashSet<Position> { (3, 6), (5, 6), (7, 2), (7, 4), (7, 5), (8, 2), (8, 5), (8, 7) }
            };
            // row fish sashimi x-wing pattern (same as above but with extra fin on top of regular position, now only one set of affected cells)
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (4, 3), (4, 8), (6, 3), (6, 6), (6, 8) },
                new HashSet<Position> { (7, 8), (8, 8) },
                new HashSet<Position> { (3, 6), (5, 6), (7, 2), (7, 4), (7, 5), (8, 2), (8, 5), (8, 7) }
            };
            // column fish x-wing pattern
            yield return new object[]
            {
                Technique.Fish.TwoColumn(),
                new HashSet<Position> { (1, 1), (1, 7), (7, 1), (7, 7) },
                new HashSet<Position> { (1, 4), (1, 6), (7, 0), (7, 4) },
                new HashSet<Position> { (2, 0), (2, 6), (4, 0), (4, 4), (4, 6) }
            };
            // column fish sashimi x-wing pattern
            yield return new object[]
            {
                Technique.Fish.TwoColumn(),
                new HashSet<Position> { (1, 1), (2, 7), (7, 1), (7, 7) },
                new HashSet<Position> { (1, 6), (2, 0) },
                new HashSet<Position> { (1, 4), (2, 6), (4, 0), (4, 4), (4, 6), (7, 0), (7, 4) }
            };
            // row fish x-wing with two fins
            yield return new object[]
            {
                Technique.Fish.TwoRow(),
                new HashSet<Position> { (4, 1), (4, 4), (7, 1), (7, 3), (7, 4), (7, 5) },
                new HashSet<Position> { (6, 4), (8, 4) },
                new HashSet<Position> { (1, 1), (5, 4), (6, 3) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeRow(),
                new HashSet<Position> { (1, 1), (1, 7), (4, 1), (4, 4), (7, 4), (7, 6) },
                new HashSet<Position> { (0, 6), (6, 7), (8, 7) },
                new HashSet<Position> { (0, 4), (0, 8), (3, 1), (3, 4), (3, 6), (3, 7), (3, 8) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeRow(),
                new HashSet<Position> { (1, 1), (1, 7), (4, 1), (4, 4), (4, 7), (7, 4), (7, 6) },
                new HashSet<Position> { (6, 7), (8, 7) },
                new HashSet<Position> { (0, 4), (0, 6), (0, 8), (3, 1), (3, 4), (3, 6), (3, 7), (3, 8) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeRow(),
                new HashSet<Position> { (2, 0), (2, 2), (4, 0), (4, 8), (6, 0), (6, 2), (6, 6), (6, 8) },
                new HashSet<Position> { (7, 8), (8, 8) },
                new HashSet<Position> { (7, 0), (7, 2), (7, 5), (7, 6), (7, 7), (8, 0), (8, 2), (8, 5), (8, 6), (8, 7) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeRow(),
                new HashSet<Position> { (2, 0), (2, 2), (4, 0), (4, 8), (6, 0), (6, 2), (6, 6), (6, 8) },
                new HashSet<Position> { (7, 8) },
                new HashSet<Position> { (3, 0), (3, 2), (3, 3), (3, 6), (3, 8), (7, 0), (7, 1), (7, 2), (7, 6) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeColumn(),
                new HashSet<Position> { (1, 0), (1, 4), (2, 0), (2, 4), (2, 8), (3, 0), (4, 0), (4, 4), (4, 8) },
                new HashSet<Position> { (4, 1) },
                new HashSet<Position> { (0, 6), (1, 6), (2, 1), (2, 5), (2, 7), (3, 1), (3, 5), (3, 6), (3, 7), (4, 6), (4, 7), (5, 5), (5, 6), (7, 6), (7, 7) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeColumn(),
                new HashSet<Position> { (1, 0), (1, 4), (2, 0), (2, 4), (2, 8), (3, 0), (4, 0), (4, 4), (4, 8), (5, 0) },
                new HashSet<Position> { (4, 1) },
                new HashSet<Position> { (0, 6), (1, 6), (2, 1), (2, 5), (2, 7), (3, 1), (3, 5), (3, 6), (3, 7), (4, 6), (4, 7), (5, 5), (5, 6), (7, 6), (7, 7) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeColumn(),
                new HashSet<Position> { (1, 0), (1, 4), (2, 0), (2, 4), (2, 8), (4, 0), (4, 4), (4, 8) },
                new HashSet<Position> { (1, 6), (2, 1), (2, 5), (2, 7), (4, 1), (4, 6), (4, 7) },
                new HashSet<Position> { (0, 6), (3, 1), (3, 5), (3, 6), (3, 7), (5, 5), (5, 6), (7, 6), (7, 7) }
            };
            yield return new object[]
            {
                Technique.Fish.ThreeColumn(),
                new HashSet<Position> { (2, 1), (2, 7), (2, 8), (3, 1), (4, 1), (4, 7), (4, 8), (8, 7), (8, 8) },
                new HashSet<Position> { (4, 2) },
                new HashSet<Position> { (0, 2), (0, 3), (0, 5), (0, 6), (1, 2), (1, 6), (2, 1), (2, 5), (2, 7), (2, 8), (3, 5), (3, 6), (4, 6), (5 ,3), (5, 5), (5, 6) }
            };
            yield return new object[]
            {
                Technique.Fish.FourRow(),
                new HashSet<Position> { (1, 1), (1, 2), (1, 7), (2, 2), (2, 7), (5, 1), (5, 5), (5, 7), (6, 1), (6, 4), (6, 7) },
                new HashSet<Position> { (3, 4), (4, 4), (7, 5), (8, 5) },
                new HashSet<Position> { }
            };
            yield return new object[]
            {
                Technique.Fish.FourRow(),
                new HashSet<Position> { (1, 1), (1, 5), (2, 5), (2, 7), (5, 3), (5, 7), (6, 1), (6, 3) },
                new HashSet<Position> { (0, 1), (3, 5), (7, 3), (8, 7) },
                new HashSet<Position> { (4, 4)}
            };
        }

        [Theory]
        [MemberData(nameof(FishTechnique_BoardStates))]
        internal void GetPossibleBoardStateChange_FishTechnique(
            FishTechnique fishTechnique,
            HashSet<Position> fishPositions,
            HashSet<Position> positionsAffected,
            HashSet<Position> positionsNotAffected)
        {
            // set up board
            int candidateValue = 7;
            var board = BoardFactory.CandidateBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !positionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(removals));

            // use fish technique
            var boardChange = fishTechnique.GetPossibleBoardStateChange(board);

            // validate
            Assert.Equal(positionsAffected, boardChange.Change.CandidatesAffected.Select(r => r.Position).ToHashSet());
        }
    }
}