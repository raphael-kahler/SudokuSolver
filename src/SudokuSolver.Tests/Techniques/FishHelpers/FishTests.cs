using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;
using SudokuSolver.Techniques.FishHelpers;
using SudokuSolver.Techniques.Helpers;
using Xunit;

namespace SudokuSolver.Tests
{
    public class FishTests
    {
        [Fact]
        public void DetermineCandidateRemovals_RowFish_SashimiXWingPattern()
        {
            // settings
            int candidateValue = 7;
            var fishPositions = new HashSet<Position> { new(2, 4), new(2, 8), new(4, 4), new(4, 8) };
            var positionsAffected = new HashSet<Position> { new(1, 7), new(3, 8), new(5, 8) };
            var posistionsNotAffected = new HashSet<Position> { new(0, 2), new(0, 4), new(0, 5), new(1, 6), new(8, 7), new(8, 8) };

            // set up board
            var board = BoardFactory.EmptyBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !posistionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals, NotFound.Instance, NoChangeDescription.Instance));

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
            var fishPositions = new HashSet<Position> { new(1, 1), new(2, 7), new(7, 1), new(7, 7) };
            var positionsAffected = new HashSet<Position> { new(1, 6), new(2, 0) };
            var posistionsNotAffected = new HashSet<Position> { new(2, 6), new(4, 0), new(4, 6), new(7, 0) };

            // set up board
            var board = BoardFactory.EmptyBoard();
            var removals = board.Cells
                .Where(c => !fishPositions.Contains(c.Position) && !positionsAffected.Contains(c.Position) && !posistionsNotAffected.Contains(c.Position))
                .Select(c => new Candidate(c.Position, candidateValue))
                .ToList();
            board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals, NotFound.Instance, NoChangeDescription.Instance));

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
                fins: ImmutableHashSet<Position>.Empty.Add(new(4, 3)).Add(new(4, 5)));

            var connects = fin.ConnectsTo(new Position(row, col));

            Assert.Equal(shouldConnect, connects);
        }
   }
}