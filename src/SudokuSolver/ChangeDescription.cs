using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public interface IChangeDescription
    {
        ISolverTechniqueDescription FoundBy { get; }
        IChangeHinter ChangeHinter { get; }
        IBoardStateChange Change { get; }
    }

    public record ChangeDescription(IBoardStateChange Change, IChangeHinter ChangeHinter, ISolverTechniqueDescription FoundBy)
        : IChangeDescription
    {
    }

    public class NoChangeDescription : IChangeDescription
    {
        public ISolverTechniqueDescription FoundBy => NotFound.Instance;
        public IChangeHinter ChangeHinter => NoHints.Instance;
        public IBoardStateChange Change => BoardStateNoChange.Instance;

        public BoardState ApplyTo(BoardState board) => board;
        private NoChangeDescription() { }
        public static NoChangeDescription Instance { get; } = new NoChangeDescription();
    }
}
