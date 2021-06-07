using System.Collections.Generic;
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

        private NoChangeDescription() { }
        public static NoChangeDescription Instance { get; } = new NoChangeDescription();
    }

    public record CombinationChangeDescription(IReadOnlyCollection<IChangeDescription> changeDescriptions)
        : IChangeDescription
    {
        public ISolverTechniqueDescription FoundBy => new CombinedTechnique(changeDescriptions.Select(c => c.FoundBy).ToList());
        public IChangeHinter ChangeHinter => NoHints.Instance;
        public IBoardStateChange Change => new BoardStateChangeCombination(changeDescriptions.Select(c => c.Change).ToList());
    }
}
