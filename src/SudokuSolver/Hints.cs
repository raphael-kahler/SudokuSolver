using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public interface IChangeHinter
    {
        IEnumerable<ChangeHint> GetHints();
    }

    public class NoHints : IChangeHinter
    {
        public IEnumerable<ChangeHint> GetHints() => Enumerable.Empty<ChangeHint>();

        private NoHints() {}
        public static NoHints Instance { get; } = new NoHints();
    }

    public record ChangeHint(string Description, IBoardStateChange ChangeDescription)
    {
        public ChangeHint(string description) : this(description, BoardStateNoChange.Instance)
        { }
    }
}