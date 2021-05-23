using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IChangeDescription
    {
        bool HasEffect { get; }
        IImmutableSet<Position> ValuesCausingChange { get; }
        IImmutableSet<Candidate> CandidatesCausingChange { get; }

        IImmutableSet<Cell> ValuesAffected { get; }
        IImmutableSet<Candidate> CandidatesAffected { get; }

        bool RelatedToRow(int row);
        bool RelatedToPosition(Position position);
    }

    public record ChangeDescription(
        IImmutableSet<Position> ValuesCausingChange,
        IImmutableSet<Candidate> CandidatesCausingChange,
        IImmutableSet<Cell> ValuesAffected,
        IImmutableSet<Candidate> CandidatesAffected)
        : IChangeDescription
    {
        public static ChangeDescription ValueSetter(ImmutableHashSet<Candidate> candidatesCausingChange, Cell valueAffected) =>
            new ChangeDescription(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty.Add(valueAffected), ImmutableHashSet<Candidate>.Empty);

        public static IChangeDescription ValuesRemovingCandidates(ImmutableHashSet<Position> valuesCausingChange, ImmutableHashSet<Candidate> canidatesAffected) =>
            canidatesAffected.Any()
                ? new ChangeDescription(valuesCausingChange, ImmutableHashSet<Candidate>.Empty, ImmutableHashSet<Cell>.Empty, canidatesAffected)
                : NoChangeDescription.Instance;

        public static IChangeDescription CandidatesRemovingCandidates(ImmutableHashSet<Candidate> candidatesCausingChange, ImmutableHashSet<Candidate> canidatesAffected) =>
            canidatesAffected.Any()
                ? new ChangeDescription(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty, canidatesAffected)
                : NoChangeDescription.Instance;

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
    }

    public class NoChangeDescription : IChangeDescription
    {
        private NoChangeDescription() { }
        public static NoChangeDescription Instance { get; } = new NoChangeDescription();

        public IImmutableSet<Position> ValuesCausingChange => ImmutableHashSet<Position>.Empty;
        public IImmutableSet<Candidate> CandidatesCausingChange => ImmutableHashSet<Candidate>.Empty;
        public IImmutableSet<Cell> ValuesAffected => ImmutableHashSet<Cell>.Empty;
        public IImmutableSet<Candidate> CandidatesAffected => ImmutableHashSet<Candidate>.Empty;
        public bool HasEffect => false;

        public bool RelatedToRow(int row) => false;
        public bool RelatedToPosition(Position position) => false;
    }
}
