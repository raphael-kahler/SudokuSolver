using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.FishTechniques;
using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests.Techniques.FishTechniques
{
    public class FishTests
    {
        [Fact]
        public void DetermineCandidateRemovals_RowFish_SashimiXWingPattern()
        {
            // settings
            int candidateValue = 7;
            var fishPositions = new HashSet<Position> { (2, 4), (2, 8), (4, 4), (4, 8) };
            var positionsAffected = new HashSet<Position> { (1, 7), (3, 8), (5, 8) };
            var posistionsNotAffected = new HashSet<Position> { (0, 2), (0, 4), (0, 5), (1, 6), (8, 7), (8, 8) };

            // set up board
            var board = BoardFactory.CandidateBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !posistionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(removals));

            // set up fish
            var fish = new FinnedFish(
                ImmutableList<FishCorner>.Empty
                    .Add(new FishCorner(new Position(2, 4)))
                    .Add(new FishCorner(new Position(2, 8)))
                    .Add(new FishCorner(new Position(4, 4))),
                new FishFin(new Position(4, 8), ImmutableHashSet<Position>.Empty.Add(new Position(4, 7))),
                RowOrientation.Instance);

            // get candidate removals
            var candidateRemovals = fish.DetermineCandidateRemovals(board, value: candidateValue).ToList();

            Assert.Equal(positionsAffected, candidateRemovals.Select(r => r.Position).ToHashSet());
        }

        [Fact]
        public void DetermineCandidateRemovals_ColumnFish_SashimiXWingPattern()
        {
            // settings
            int candidateValue = 7;
            var fishPositions = new HashSet<Position> { (1, 1), (2, 7), (7, 1), (7, 7) };
            var positionsAffected = new HashSet<Position> { (1, 6), (2, 0) };
            var posistionsNotAffected = new HashSet<Position> { (2, 6), (4, 0), (4, 6), (7, 0) };

            // set up board
            var board = BoardFactory.CandidateBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !posistionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(BoardStateChange.RemoveCandidates(removals));

            // set up fish
            var fish = new FinnedFish(
                ImmutableList<FishCorner>.Empty
                    .Add(new FishCorner(new Position(1, 1)))
                    .Add(new FishCorner(new Position(7, 1)))
                    .Add(new FishCorner(new Position(7, 7))),
                new FishFin(new Position(1, 7), ImmutableHashSet<Position>.Empty.Add(new Position(2, 7))),
                ColumnOrientation.Instance);

            // get candidate removals
            var candidateRemovals = fish.DetermineCandidateRemovals(board, value: candidateValue).ToList();

            Assert.Equal(positionsAffected, candidateRemovals.Select(r => r.Position).ToHashSet());
        }

        [Theory]
        [InlineData(true, 3, 3)]
        [InlineData(true, 5, 4)]
        [InlineData(false, 7, 3)]
        [InlineData(false, 7, 5)]
        public void Fin_ConnectsTo(bool shouldConnect, int row, int col)
        {
            var fin = new FishFin(
                position: new Position(4, 3),
                fins: ImmutableHashSet<Position>.Empty.Add((4, 3)).Add((4, 5)));

            var connects = fin.ConnectsTo(new Position(row, col));

            Assert.Equal(shouldConnect, connects);
        }
   }
}