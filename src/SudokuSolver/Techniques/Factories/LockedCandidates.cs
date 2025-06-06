using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.LockedCandidates;

namespace SudokuSolver.Techniques.Factories;

public class LockedCandidatesTechniqueFactory
{
    internal LockedCandidatesTechniqueFactory() { }

    public LockedCandidatesClaimingFactory Claiming => new LockedCandidatesClaimingFactory();
    public LockedCandidatesPointingFactory Pointing => new LockedCandidatesPointingFactory();
}

public class LockedCandidatesClaimingFactory
{
    internal LockedCandidatesClaimingFactory() { }

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
