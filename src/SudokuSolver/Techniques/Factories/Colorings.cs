using SudokuSolver.Techniques.Coloring;

namespace SudokuSolver.Techniques.Factories
{
    public class ColoringsFactory
    {
        internal ColoringsFactory() { }

        public ISolverTechnique SimpleColoring() => new SimpleColorsTechnique();
    }
}