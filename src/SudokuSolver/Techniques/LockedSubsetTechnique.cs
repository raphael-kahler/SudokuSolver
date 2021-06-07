using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.LockedSubsetTechniques;

namespace SudokuSolver.Techniques
{
    public static class LockedSubsets
    {
        public static class LockedCandidateClaiming
        {
            public static ISolverTechnique Row() => new LockedCandidatesClaimingTechnique(RowCellCollector.Instance);
            public static ISolverTechnique Column() => new LockedCandidatesClaimingTechnique(ColumnCellCollector.Instance);
            public static IEnumerable<ISolverTechnique> AllDirections() => new List<ISolverTechnique> { Row(), Column() };
        }

        public static class LockedCandidatesPointing
        {
            public static ISolverTechnique Row() => new LockedCandidatesPointingTechnique(RowOrientation.Instance);
            public static ISolverTechnique Column() => new LockedCandidatesPointingTechnique(ColumnOrientation.Instance);
            public static IEnumerable<ISolverTechnique> AllDirections() => new List<ISolverTechnique> { Row(), Column() };
        }
    }
}