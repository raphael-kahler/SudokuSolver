namespace SudokuSolver.Techniques
{
    public class NotFound : ISolverTechniqueDescription
    {
        private NotFound() { }
        public static NotFound Instance { get; } = new NotFound();
        public string Description => "No change found";
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Undefined;
    }
}