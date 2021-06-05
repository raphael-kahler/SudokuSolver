using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IChangeHinter
    {
        IEnumerable<IChangeHint> GetHints();
    }

    public class NoHints : IChangeHinter
    {
        public IEnumerable<IChangeHint> GetHints() => Enumerable.Empty<IChangeHint>();

        private NoHints() {}
        public static NoHints Instance { get; } = new NoHints();
    }

    public interface IChangeHint
    {
        string Description { get; }
        IImmutableSet<Cell> HintedCells { get; }
        IImmutableSet<Candidate> HintedCandidates { get; }
    }

    public record ChangeHint(string Description, IImmutableSet<Cell> HintedCells, IImmutableSet<Candidate> HintedCandidates)
        : IChangeHint
    {
        public ChangeHint(string description) : this(description, ImmutableHashSet<Cell>.Empty, ImmutableHashSet<Candidate>.Empty)
        { }

        public ChangeHint(string description, IImmutableSet<Cell> hintedCells) : this(description, hintedCells, ImmutableHashSet<Candidate>.Empty)
        { }

        public ChangeHint(string description, IImmutableSet<Candidate> hintedCandidates) : this(description, ImmutableHashSet<Cell>.Empty, hintedCandidates)
        { }
    }
}