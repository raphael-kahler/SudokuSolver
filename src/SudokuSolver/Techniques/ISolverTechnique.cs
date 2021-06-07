using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Techniques
{
    public interface ISolverTechniqueDescription
    {
        public string TechniqueName => "";
        string Description { get; }
        DifficultyLevel DifficultyLevel { get; }
    }

    public enum DifficultyLevel
    {
        Undefined,
        Trivial,
        Easy,
        Medium,
        Advanced,
        Expert
    }

    public interface ISolverTechnique : ISolverTechniqueDescription
    {
        IChangeDescription GetPossibleBoardStateChange(BoardState board);
    }

    public record CombinedTechnique(IReadOnlyCollection<ISolverTechniqueDescription> Techniques)
        : ISolverTechniqueDescription
    {
        public string TechniqueName => "Combined Technique";

        public string Description => string.Join(Environment.NewLine,
            Techniques
                .OrderBy(c => c.DifficultyLevel)
                .Select(c => c.Description)
                .Distinct());

        public DifficultyLevel DifficultyLevel =>
            Techniques.Select(c => c.DifficultyLevel).Max();
    }

}