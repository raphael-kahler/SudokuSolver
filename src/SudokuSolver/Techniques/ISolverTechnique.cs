namespace SudokuSolver.Techniques;

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

internal class NotFound : ISolverTechniqueDescription
{
    private NotFound() { }
    public static NotFound Instance { get; } = new NotFound();
    public string Description => "No change found";
    public DifficultyLevel DifficultyLevel => DifficultyLevel.Undefined;
}

internal record CombinedTechnique(IReadOnlyCollection<ISolverTechniqueDescription> Techniques)
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
