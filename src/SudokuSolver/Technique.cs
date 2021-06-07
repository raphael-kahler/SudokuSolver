using SudokuSolver.Techniques.Factories;

namespace SudokuSolver
{
    public static class Technique
    {
        public static EliminationByValueFactory EliminationByValue => new EliminationByValueFactory();
        public static SubsetTechniqueFactory Subsets => new SubsetTechniqueFactory();
        public static LockedSubsetTechniqueFactory LockedSubsets => new LockedSubsetTechniqueFactory();
        public static FishTechniqueFactory Fish => new FishTechniqueFactory();
        public static WingTechniqueFactory Wings => new WingTechniqueFactory();
    }
}