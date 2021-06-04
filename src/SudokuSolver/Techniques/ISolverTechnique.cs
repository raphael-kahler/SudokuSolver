using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Techniques
{
    public interface IChangeFinder
    {
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

    public interface ISolverTechnique : IChangeFinder
    {
        IBoardStateChange GetPossibleBoardStateChange(BoardState board);
    }

    public record FoundByCombination(IReadOnlyCollection<IChangeFinder> ChangeFinders)
        : IChangeFinder
    {
        public string Description => string.Join(Environment.NewLine,
            ChangeFinders
                .OrderBy(c => c.DifficultyLevel)
                .Select(c => c.Description)
                .Distinct());

        public DifficultyLevel DifficultyLevel =>
            ChangeFinders.Select(c => c.DifficultyLevel).Max();
    }

}