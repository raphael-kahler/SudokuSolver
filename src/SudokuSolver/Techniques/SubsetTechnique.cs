using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.SubsetTechniques;

namespace SudokuSolver.Techniques
{
    public static class Subsets
    {
        public static ISolverTechnique NakedPairRow() => new NakedSubset(2, RowCellCollector.Instance);
        public static ISolverTechnique NakedPairColumn() => new NakedSubset(2, ColumnCellCollector.Instance);
        public static ISolverTechnique NakedPairBox() => new NakedSubset(2, BoxCellCollector.Instance);
        public static ISolverTechnique NakedTripleRow() => new NakedSubset(3, RowCellCollector.Instance);
        public static ISolverTechnique NakedTripleColumn() => new NakedSubset(3, ColumnCellCollector.Instance);
        public static ISolverTechnique NakedTripleBox() => new NakedSubset(3, BoxCellCollector.Instance);
        public static ISolverTechnique NakedQuadRow() => new NakedSubset(4, RowCellCollector.Instance);
        public static ISolverTechnique NakedQuadColumn() => new NakedSubset(4, ColumnCellCollector.Instance);
        public static ISolverTechnique NakedQuadBox() => new NakedSubset(4, BoxCellCollector.Instance);
        public static ISolverTechnique NakedSingle() => new NakedSingle();
        public static IEnumerable<ISolverTechnique> NakedPairs() => new List<ISolverTechnique> { NakedPairRow(), NakedPairColumn(), NakedPairBox() };
        public static IEnumerable<ISolverTechnique> NakedTriples() => new List<ISolverTechnique> { NakedTripleRow(), NakedTripleColumn(), NakedTripleBox() };
        public static IEnumerable<ISolverTechnique> NakedQuads() => new List<ISolverTechnique> { NakedQuadRow(), NakedQuadColumn(), NakedQuadBox() };

        public static ISolverTechnique HiddenSingleRow() => new HiddenSubset(1, RowCellCollector.Instance);
        public static ISolverTechnique HiddenSingleColumn() => new HiddenSubset(1, ColumnCellCollector.Instance);
        public static ISolverTechnique HiddenSingleBox() => new HiddenSubset(1, BoxCellCollector.Instance);
        public static ISolverTechnique HiddenPairRow() => new HiddenSubset(2, RowCellCollector.Instance);
        public static ISolverTechnique HiddenPairColumn() => new HiddenSubset(2, ColumnCellCollector.Instance);
        public static ISolverTechnique HiddenPairBox() => new HiddenSubset(2, BoxCellCollector.Instance);
        public static ISolverTechnique HiddenTripleRow() => new HiddenSubset(3, RowCellCollector.Instance);
        public static ISolverTechnique HiddenTripleColumn() => new HiddenSubset(3, ColumnCellCollector.Instance);
        public static ISolverTechnique HiddenTripleBox() => new HiddenSubset(3, BoxCellCollector.Instance);
        public static ISolverTechnique HiddenQuadRow() => new HiddenSubset(4, RowCellCollector.Instance);
        public static ISolverTechnique HiddenQuadColumn() => new HiddenSubset(4, ColumnCellCollector.Instance);
        public static ISolverTechnique HiddenQuadBox() => new HiddenSubset(4, BoxCellCollector.Instance);
        public static IEnumerable<ISolverTechnique> HiddenSingles() => new List<ISolverTechnique> { HiddenSingleRow(), HiddenSingleColumn(), HiddenSingleBox() };
        public static IEnumerable<ISolverTechnique> HiddenPairs() => new List<ISolverTechnique> { HiddenPairRow(), HiddenPairColumn(), HiddenPairBox() };
        public static IEnumerable<ISolverTechnique> HiddenTriples() => new List<ISolverTechnique> { HiddenTripleRow(), HiddenTripleColumn(), HiddenTripleBox() };
        public static IEnumerable<ISolverTechnique> HiddenQuads() => new List<ISolverTechnique> { HiddenQuadRow(), HiddenQuadColumn(), HiddenQuadBox() };
    }
}