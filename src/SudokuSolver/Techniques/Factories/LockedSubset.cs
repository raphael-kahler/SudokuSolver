using System.Collections.Generic;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.LockedSubsetTechniques;

namespace SudokuSolver.Techniques.Factories
{
    public class LockedSubsetTechniqueFactory
    {
        internal LockedSubsetTechniqueFactory() { }

        public LockedCandidateClaimingFactory LockedCandidateClaiming => new LockedCandidateClaimingFactory();
        public LockedCandidatesPointingFactory LockedCandidatesPointing => new LockedCandidatesPointingFactory();
    }

    public class LockedCandidateClaimingFactory
    {
        internal LockedCandidateClaimingFactory() { }

        public ISolverTechnique Row() => new LockedCandidatesClaimingTechnique(RowCellCollector.Instance);
        public ISolverTechnique Column() => new LockedCandidatesClaimingTechnique(ColumnCellCollector.Instance);
        public IEnumerable<ISolverTechnique> AllDirections() => new List<ISolverTechnique> { Row(), Column() };
    }

    public class LockedCandidatesPointingFactory
    {
        internal LockedCandidatesPointingFactory() { }

        public ISolverTechnique Row() => new LockedCandidatesPointingTechnique(RowOrientation.Instance);
        public ISolverTechnique Column() => new LockedCandidatesPointingTechnique(ColumnOrientation.Instance);
        public IEnumerable<ISolverTechnique> AllDirections() => new List<ISolverTechnique> { Row(), Column() };
    }
}