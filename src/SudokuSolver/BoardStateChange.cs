using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IBoardStateChange
    {
        bool HasEffect { get; }
        IImmutableSet<Position> ValuesCausingChange { get; }
        IImmutableSet<Candidate> CandidatesCausingChange { get; }

        IImmutableSet<Cell> ValuesAffected { get; }
        IImmutableSet<Candidate> CandidatesAffected { get; }

        bool RelatedToRow(int row);
        bool RelatedToPosition(Position position);
        BoardState ApplyTo(BoardState board);
    }

    public record BoardStateChange(
        IImmutableSet<Position> ValuesCausingChange,
        IImmutableSet<Candidate> CandidatesCausingChange,
        IImmutableSet<Cell> ValuesAffected,
        IImmutableSet<Candidate> CandidatesAffected)
        : IBoardStateChange
    {
        public static BoardStateChange ValueSetter(ImmutableHashSet<Candidate> candidatesCausingChange, Cell valueAffected) =>
            new BoardStateChange(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty.Add(valueAffected), ImmutableHashSet<Candidate>.Empty);

        public static IBoardStateChange ValuesRemovingCandidates(ImmutableHashSet<Position> valuesCausingChange, ImmutableHashSet<Candidate> canidatesAffected) =>
            canidatesAffected.Any()
                ? new BoardStateChange(valuesCausingChange, ImmutableHashSet<Candidate>.Empty, ImmutableHashSet<Cell>.Empty, canidatesAffected)
                : BoardStateNoChange.Instance;

        public static IBoardStateChange CandidatesRemovingCandidates(ImmutableHashSet<Candidate> candidatesCausingChange, ImmutableHashSet<Candidate> canidatesAffected) =>
            canidatesAffected.Any()
                ? new BoardStateChange(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty, canidatesAffected)
                : BoardStateNoChange.Instance;

        public static IBoardStateChange ForCandidatesCausingChange(ImmutableHashSet<Candidate> candidatesCausingChange) =>
            new BoardStateChange(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty, ImmutableHashSet<Candidate>.Empty);

        public static IBoardStateChange NoChange() => BoardStateNoChange.Instance;

        public static BoardStateChange SetCell(Cell cell) =>
            new BoardStateChange(
                ImmutableHashSet<Position>.Empty,
                ImmutableHashSet<Candidate>.Empty,
                ImmutableHashSet<Cell>.Empty.Add(cell),
                ImmutableHashSet<Candidate>.Empty);

        public static BoardStateChange SetCell(Position position, int value) => SetCell(Cell.WithValue(position, value));
        public static BoardStateChange ClearCell(Position position) => SetCell(Cell.Empty(position));

        public static BoardStateChange RemoveCandidates(IEnumerable<Candidate> candidateRemovals) =>
            new BoardStateChange(
                ImmutableHashSet<Position>.Empty,
                ImmutableHashSet<Candidate>.Empty,
                ImmutableHashSet<Cell>.Empty,
                candidateRemovals.ToImmutableHashSet());

        public bool HasEffect => ValuesAffected.Any() || CandidatesAffected.Any();

        public bool RelatedToRow(int row) =>
            ValuesCausingChange.Any(p => p.Row == row) ||
            CandidatesCausingChange.Any(c => c.Position.Row == row) ||
            ValuesAffected.Any(v => v.Position.Row == row) ||
            CandidatesAffected.Any(c => c.Position.Row == row);

        public bool RelatedToPosition(Position position) =>
            ValuesCausingChange.Any(p => p == position) ||
            CandidatesCausingChange.Any(c => c.Position == position) ||
            ValuesAffected.Any(v => v.Position == position) ||
            CandidatesAffected.Any(c => c.Position == position);

        public BoardState ApplyTo(BoardState board)
        {
            var cells = board.Cells;
            foreach (var c in CandidatesAffected)
            {
                var oldCell = cells.Single(cell => cell.Position == c.Position);
                var newCell = oldCell with { Candidates = oldCell.Candidates.Remove(c.CandidateValue) };
                cells = cells.Replace(oldCell, newCell);
            }
            foreach (var newCell in ValuesAffected)
            {
                var oldCell = cells.Single(cell => cell.Position == newCell.Position);
                cells = cells.Replace(oldCell, newCell);
            }
            return new BoardState(cells);
        }
    }

    public class BoardStateNoChange : IBoardStateChange
    {
        private BoardStateNoChange() { }
        public static BoardStateNoChange Instance { get; } = new BoardStateNoChange();
        public IImmutableSet<Position> ValuesCausingChange => ImmutableHashSet<Position>.Empty;
        public IImmutableSet<Candidate> CandidatesCausingChange => ImmutableHashSet<Candidate>.Empty;
        public IImmutableSet<Cell> ValuesAffected => ImmutableHashSet<Cell>.Empty;
        public IImmutableSet<Candidate> CandidatesAffected => ImmutableHashSet<Candidate>.Empty;
        public bool HasEffect => false;


        public bool RelatedToRow(int row) => false;
        public bool RelatedToPosition(Position position) => false;
        public BoardState ApplyTo(BoardState board) => board;
    }

    public record BoardStateChangeCombination(IReadOnlyCollection<IBoardStateChange> ChangeDescriptions)
        : IBoardStateChange
    {
        public bool HasEffect => ChangeDescriptions.Any(c => c.HasEffect);

        public IImmutableSet<Position> ValuesCausingChange =>
            ChangeDescriptions.SelectMany(c => c.ValuesCausingChange).ToImmutableHashSet();

        public IImmutableSet<Candidate> CandidatesCausingChange =>
            ChangeDescriptions.SelectMany(c => c.CandidatesCausingChange).ToImmutableHashSet();

        public IImmutableSet<Cell> ValuesAffected =>
            ChangeDescriptions.SelectMany(c => c.ValuesAffected).ToImmutableHashSet();

        public IImmutableSet<Candidate> CandidatesAffected =>
            ChangeDescriptions.SelectMany(c => c.CandidatesAffected).ToImmutableHashSet();

        public bool RelatedToPosition(Position position) =>
            ChangeDescriptions.Any(c => c.RelatedToPosition(position));

        public bool RelatedToRow(int row) =>
            ChangeDescriptions.Any(c => c.RelatedToRow(row));

        public BoardState ApplyTo(BoardState board)
        {
            foreach (var change in ChangeDescriptions)
            {
                board = change.ApplyTo(board);
            }
            return board;
        }
    }
}
