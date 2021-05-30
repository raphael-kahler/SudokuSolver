using SudokuSolver.Techniques.Wings;

namespace SudokuSolver.Techniques
{
    public static class WingTechnique
    {
        public static XyWingTechnique XyWing() => new XyWingTechnique();
        public static XyzWingTechnique XyzWing() => new XyzWingTechnique();
        public static WxyzWingTechnique WxyzWing() => new WxyzWingTechnique();
    }
}