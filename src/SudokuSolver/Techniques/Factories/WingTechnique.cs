using SudokuSolver.Techniques.Wings;

namespace SudokuSolver.Techniques.Factories;

public class WingTechniqueFactory
{
    public ISolverTechnique XyWing() => new XyWingTechnique();
    public ISolverTechnique XyzWing() => new XyzWingTechnique();
    public ISolverTechnique WxyzWing() => new WxyzWingTechnique();
}
