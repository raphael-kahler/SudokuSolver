using System.Collections.Generic;
using SudokuSolver.Techniques.FishTechniques;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Factories
{
    public class FishTechniqueFactory
    {
        public ISolverTechnique TwoRow() => new FishTechnique(2, RowOrientation.Instance);
        public ISolverTechnique TwoColumn() => new FishTechnique(2, ColumnOrientation.Instance);
        public ISolverTechnique ThreeRow() => new FishTechnique(3, RowOrientation.Instance);
        public ISolverTechnique ThreeColumn() => new FishTechnique(3, ColumnOrientation.Instance);
        public ISolverTechnique FourRow() => new FishTechnique(4, RowOrientation.Instance);
        public ISolverTechnique FourColumn() => new FishTechnique(4, ColumnOrientation.Instance);

        public IEnumerable<ISolverTechnique> TwoFish() => new List<ISolverTechnique> { TwoRow(), TwoColumn() };
        public IEnumerable<ISolverTechnique> ThreeFish() => new List<ISolverTechnique> { ThreeRow(), ThreeColumn() };
        public IEnumerable<ISolverTechnique> FourFish() => new List<ISolverTechnique> { FourRow(), FourColumn() };
        public IEnumerable<ISolverTechnique> XWing() => TwoFish();
        public IEnumerable<ISolverTechnique> Swordfish() => ThreeFish();
        public IEnumerable<ISolverTechnique> Jellyfish() => FourFish();
    }
}