using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.SubsetTechniques;

namespace SudokuSolver.Techniques.Factories
{
    public class SubsetTechniqueFactory
    {
        internal SubsetTechniqueFactory() { }

        public ISolverTechnique NakedPairRow() => new NakedSubset(2, RowCellCollector.Instance);
        public ISolverTechnique NakedPairColumn() => new NakedSubset(2, ColumnCellCollector.Instance);
        public ISolverTechnique NakedPairBox() => new NakedSubset(2, BoxCellCollector.Instance);
        public ISolverTechnique NakedTripleRow() => new NakedSubset(3, RowCellCollector.Instance);
        public ISolverTechnique NakedTripleColumn() => new NakedSubset(3, ColumnCellCollector.Instance);
        public ISolverTechnique NakedTripleBox() => new NakedSubset(3, BoxCellCollector.Instance);
        public ISolverTechnique NakedQuadRow() => new NakedSubset(4, RowCellCollector.Instance);
        public ISolverTechnique NakedQuadColumn() => new NakedSubset(4, ColumnCellCollector.Instance);
        public ISolverTechnique NakedQuadBox() => new NakedSubset(4, BoxCellCollector.Instance);
        public ISolverTechnique NakedSingle() => new NakedSingle();
        public IEnumerable<ISolverTechnique> NakedPairs() => new List<ISolverTechnique> { NakedPairRow(), NakedPairColumn(), NakedPairBox() };
        public IEnumerable<ISolverTechnique> NakedTriples() => new List<ISolverTechnique> { NakedTripleRow(), NakedTripleColumn(), NakedTripleBox() };
        public IEnumerable<ISolverTechnique> NakedQuads() => new List<ISolverTechnique> { NakedQuadRow(), NakedQuadColumn(), NakedQuadBox() };

        public ISolverTechnique HiddenSingleRow() => new HiddenSubset(1, RowCellCollector.Instance);
        public ISolverTechnique HiddenSingleColumn() => new HiddenSubset(1, ColumnCellCollector.Instance);
        public ISolverTechnique HiddenSingleBox() => new HiddenSubset(1, BoxCellCollector.Instance);
        public ISolverTechnique HiddenPairRow() => new HiddenSubset(2, RowCellCollector.Instance);
        public ISolverTechnique HiddenPairColumn() => new HiddenSubset(2, ColumnCellCollector.Instance);
        public ISolverTechnique HiddenPairBox() => new HiddenSubset(2, BoxCellCollector.Instance);
        public ISolverTechnique HiddenTripleRow() => new HiddenSubset(3, RowCellCollector.Instance);
        public ISolverTechnique HiddenTripleColumn() => new HiddenSubset(3, ColumnCellCollector.Instance);
        public ISolverTechnique HiddenTripleBox() => new HiddenSubset(3, BoxCellCollector.Instance);
        public ISolverTechnique HiddenQuadRow() => new HiddenSubset(4, RowCellCollector.Instance);
        public ISolverTechnique HiddenQuadColumn() => new HiddenSubset(4, ColumnCellCollector.Instance);
        public ISolverTechnique HiddenQuadBox() => new HiddenSubset(4, BoxCellCollector.Instance);
        public IEnumerable<ISolverTechnique> HiddenSingles() => new List<ISolverTechnique> { HiddenSingleRow(), HiddenSingleColumn(), HiddenSingleBox() };
        public IEnumerable<ISolverTechnique> HiddenPairs() => new List<ISolverTechnique> { HiddenPairRow(), HiddenPairColumn(), HiddenPairBox() };
        public IEnumerable<ISolverTechnique> HiddenTriples() => new List<ISolverTechnique> { HiddenTripleRow(), HiddenTripleColumn(), HiddenTripleBox() };
        public IEnumerable<ISolverTechnique> HiddenQuads() => new List<ISolverTechnique> { HiddenQuadRow(), HiddenQuadColumn(), HiddenQuadBox() };
    }
}