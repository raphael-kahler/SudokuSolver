using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.FishHelpers
{
    internal interface IFishPart
    {
        bool IsFin { get; }
        Position LogicalPosition { get; }
        IEnumerable<Position> PhysicalPositions { get; }
        bool ConnectsTo(Position position);
    }

    internal record FishCorner(Position LogicalPosition) : IFishPart
    {
        public bool IsFin => false;
        public IEnumerable<Position> PhysicalPositions { get { yield return LogicalPosition; } }
        public bool ConnectsTo(Position position) => position.ConnectsTo(LogicalPosition);
    }

    internal record FishFin : IFishPart
    {
        public Position LogicalPosition { get; }
        public IImmutableSet<Position> Fins { get; }
        public IEnumerable<Position> PhysicalPositions => Fins;
        public bool IsFin => true;

        public FishFin(Position position, IImmutableSet<Position> fins)
        {
            LogicalPosition = position;
            Fins = fins;

            if (fins.Count < 1)
            {
                throw new ArgumentException("At least one fin position needs to be provided.", nameof(fins));
            }
            foreach (var fin in fins)
            {
                if (position.Box != fin.Box)
                {
                    throw new ArgumentException($"One of the fins {fin} is not in same box as the fish corner position {position}.", nameof(fins));
                }
                if (position.Row != fin.Row && position.Col != fin.Col)
                {
                    throw new ArgumentException($"The fin {fin} needs to be in the same row or column as the fish corner position {position}.", nameof(fins));
                }
            }
            var row = fins.First().Row;
            var col = fins.First().Col;
            if (!fins.All(f => f.Row == row) && !fins.All(f => f.Col == col))
            {
                throw new ArgumentException($"All fins need to be either be in the same row or in the same column.", nameof(fins));
            }
        }

        public bool ConnectsTo(Position position) => Fins.All(f => f.ConnectsTo(position));
    }

    internal interface IFish
    {
        bool IsFish { get; }
        IEnumerable<Candidate> DefiningCandidates(int candidateValue);
        IEnumerable<Candidate> DetermineCandidateRemovals(BoardState board, int value);
    }

    internal class NoFish : IFish
    {
        public bool IsFish => false;

        public IEnumerable<Candidate> DefiningCandidates(int candidateValue) =>
            Enumerable.Empty<Candidate>();

        public IEnumerable<Candidate> DetermineCandidateRemovals(BoardState board, int value) =>
            Enumerable.Empty<Candidate>();

        public static NoFish Instance { get; } = new NoFish();
        private NoFish() { }
    }

    internal abstract class Fish : IFish
    {
        public bool IsFish => true;

        public abstract IEnumerable<Candidate> DefiningCandidates(int candidateValue);

        public IEnumerable<Candidate> DetermineCandidateRemovals(BoardState board, int value) =>
            AffectedCells(board)
                .Where(cell => cell.Candidates.Contains(value))
                .Select(cell => new Candidate(cell.Position, value));

        protected abstract IEnumerable<Cell> AffectedCells(BoardState board);
    }

    internal class RegularFish : Fish
    {
        protected IImmutableList<FishCorner> Parts { get; }
        private IReadOnlySet<int> PrimaryValues { get; }
        private IReadOnlySet<int> SecondaryValues { get; }
        private IOrientation Orientation { get; }

        public RegularFish(IImmutableList<FishCorner> parts, IOrientation orientation)
        {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            Orientation = orientation ?? throw new ArgumentNullException(nameof(orientation));
            PrimaryValues = parts.Select(p => Orientation.PrimaryDimension( p.LogicalPosition)).ToHashSet();
            SecondaryValues = parts.Select(p => Orientation.SecondaryDimension(p.LogicalPosition)).ToHashSet();
        }

        public override IEnumerable<Candidate> DefiningCandidates(int candidateValue) =>
            Parts.SelectMany(p => p.PhysicalPositions).Select(p => new Candidate(p, candidateValue));

        protected override IEnumerable<Cell> AffectedCells(BoardState board) =>
            SecondaryValues.SelectMany(value => Orientation.CellsForSecondaryDimension(board, value))
                .Where(cell => !PrimaryValues.Contains(Orientation.PrimaryDimension(cell.Position)));
    }

    internal class FinnedFish : Fish
    {
        public IImmutableList<FishCorner> Parts { get; }
        public FishFin Fin { get; }
        private IReadOnlySet<int> PrimaryValues { get; }
        private FishCorner CornerMatchingFin { get; }
        private IOrientation Orientation { get; }

        public FinnedFish(IImmutableList<FishCorner> parts, FishFin fin, IOrientation orientation)
        {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            Fin = fin ?? throw new ArgumentNullException(nameof(fin));
            Orientation = orientation ?? throw new ArgumentNullException(nameof(orientation));
            PrimaryValues = parts.Select(p => Orientation.PrimaryDimension(p.LogicalPosition)).ToHashSet();
            CornerMatchingFin = parts.First(p => Orientation.SecondaryDimension(p.LogicalPosition) == Orientation.SecondaryDimension(fin.LogicalPosition));
        }

        public override IEnumerable<Candidate> DefiningCandidates(int candidateValue) =>
            Parts.SelectMany(p => p.PhysicalPositions)
                .Concat(Fin.PhysicalPositions)
                .Select(p => new Candidate(p, candidateValue));

        protected override IEnumerable<Cell> AffectedCells(BoardState board) =>
            AffectedSecondaryValues()
                .SelectMany(value => Orientation.CellsForSecondaryDimension(board, value))
                .Where(cell =>
                    !PrimaryValues.Contains(Orientation.PrimaryDimension(cell.Position))
                    && FinsConnect(cell.Position));

        private bool FinsConnect(Position position)
        {
            if (Parts.Count(p => Orientation.SecondaryDimension(p.LogicalPosition) == Orientation.SecondaryDimension(Fin.LogicalPosition)) == 1)
            {
                return Fin.ConnectsTo(position) && CornerMatchingFin.ConnectsTo(position);
            }
            else
            {
                return Fin.ConnectsTo(position)
                    && CornerMatchingFin.ConnectsTo(position)
                    && Orientation.SecondaryDimension(Fin.LogicalPosition) == Orientation.SecondaryDimension(position);
            }
        }

        private IEnumerable<int> AffectedSecondaryValues() =>
            Fin.Fins.Select(f => Orientation.SecondaryDimension(f)).Append(Orientation.SecondaryDimension(CornerMatchingFin.LogicalPosition));
    }
}