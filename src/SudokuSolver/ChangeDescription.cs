using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IChangeDescription
    {
        bool HasEffect { get; }
        IChangeHinter ChangeHinter { get; }
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
        IImmutableSet<Candidate> CandidatesAffected,
        IChangeHinter ChangeHinter)
        : IChangeDescription
    {
        public static ChangeDescription ValueSetter(ImmutableHashSet<Candidate> candidatesCausingChange, Cell valueAffected, IChangeHinter changeHinter) =>
            new ChangeDescription(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty.Add(valueAffected), ImmutableHashSet<Candidate>.Empty, changeHinter);

        public static IChangeDescription ValuesRemovingCandidates(ImmutableHashSet<Position> valuesCausingChange, ImmutableHashSet<Candidate> canidatesAffected, IChangeHinter changeHinter) =>
            canidatesAffected.Any()
                ? new ChangeDescription(valuesCausingChange, ImmutableHashSet<Candidate>.Empty, ImmutableHashSet<Cell>.Empty, canidatesAffected, changeHinter)
                : NoChangeDescription.Instance;

        public static IChangeDescription CandidatesRemovingCandidates(ImmutableHashSet<Candidate> candidatesCausingChange, ImmutableHashSet<Candidate> canidatesAffected, IChangeHinter changeHinter) =>
            canidatesAffected.Any()
                ? new ChangeDescription(ImmutableHashSet<Position>.Empty, candidatesCausingChange, ImmutableHashSet<Cell>.Empty, canidatesAffected, changeHinter)
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
        public IChangeHinter ChangeHinter => NoHints.Instance;
        public IImmutableSet<Position> ValuesCausingChange => ImmutableHashSet<Position>.Empty;
        public IImmutableSet<Candidate> CandidatesCausingChange => ImmutableHashSet<Candidate>.Empty;
        public IImmutableSet<Cell> ValuesAffected => ImmutableHashSet<Cell>.Empty;
        public IImmutableSet<Candidate> CandidatesAffected => ImmutableHashSet<Candidate>.Empty;
        public bool HasEffect => false;


        public bool RelatedToRow(int row) => false;
        public bool RelatedToPosition(Position position) => false;
    }

    public record ChangeDescriptionCombination(IReadOnlyCollection<IChangeDescription> ChangeDescriptions)
        : IChangeDescription
    {
        public bool HasEffect => ChangeDescriptions.Any(c => c.HasEffect);
        public IChangeHinter ChangeHinter => NoHints.Instance;

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
    }
}
