using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IChangeHinter
    {
        bool HasHints { get; }
        IEnumerable<ChangeHint> GetHints();
    }

    public class NoHints : IChangeHinter
    {
        public bool HasHints => false;
        public IEnumerable<ChangeHint> GetHints() => Enumerable.Empty<ChangeHint>();

        private NoHints() {}
        public static NoHints Instance { get; } = new NoHints();
    }

    public record ChangeHint(string Description, IChangeDescription ChangeDescription)
    {
        public ChangeHint(string description) : this(description, NoChangeDescription.Instance)
        { }
    }
}