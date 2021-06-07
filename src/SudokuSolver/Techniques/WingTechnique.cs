using SudokuSolver.Techniques.Wings;

namespace SudokuSolver.Techniques
{
    public static class WingTechnique
    {
        public static ISolverTechnique XyWing() => new XyWingTechnique();
        public static ISolverTechnique XyzWing() => new XyzWingTechnique();
        public static ISolverTechnique WxyzWing() => new WxyzWingTechnique();
    }
}