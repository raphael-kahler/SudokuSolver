using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Factories
{
    public class EliminationByValueFactory
    {
        internal EliminationByValueFactory() { }

        public ISolverTechnique Row() => new EliminationByValueTechnique(RowCellCollector.Instance);
        public ISolverTechnique Column() => new EliminationByValueTechnique(ColumnCellCollector.Instance);
        public ISolverTechnique Box() => new EliminationByValueTechnique(BoxCellCollector.Instance);
        public IEnumerable<ISolverTechnique> AllDirections() => new List<ISolverTechnique> { Row(), Column(), Box() };
    }
}