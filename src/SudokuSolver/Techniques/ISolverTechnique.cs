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
}