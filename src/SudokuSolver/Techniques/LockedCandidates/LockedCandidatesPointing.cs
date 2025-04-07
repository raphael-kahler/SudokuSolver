using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.LockedCandidates;

internal class LockedCandidatesPointingTechnique : ISolverTechnique
{
    private IOrientation Orientation { get; }
    public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
    public string Description =>
        $"In one box all candidates of a number are in the same {Orientation.PrimaryDimensionName}. " +
        $"Remove candidates from other boxes of that {Orientation.PrimaryDimensionName}.";

    internal LockedCandidatesPointingTechnique(IOrientation orientation)
    {
        Orientation = orientation ?? throw new System.ArgumentNullException(nameof(orientation));
    }

    public IChangeDescription GetPossibleBoardStateChange(BoardState board)
    {
        for (int box = 0; box < 9; ++box)
        {
            var cells = board.Box(box);
            for (int value = 1; value <= 9; ++value)
            {
                var changeDescription = GetChangeForValue(board, cells, value);
                if (changeDescription.Change.HasEffect)
                {
                    return changeDescription;
                }
            }
        }
        return NoChangeDescription.Instance;
    }

    private IChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
    {
        var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
        var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

        var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
        if (cellsForCandidate.Any())
        {
            var dimValue = Orientation.PrimaryDimension(cellsForCandidate.First().Position);
            if (cellsForCandidate.All(c => Orientation.PrimaryDimension(c.Position) == dimValue))
            {
                foreach (var cell in Orientation.CellsForPrimaryDimension(board, dimValue))
                {
                    if (cell.Candidates.Contains(value))
                    {
                        var candidate = new Candidate(cell.Position, value);
                        if (cellsForCandidate.Contains(cell))
                        {
                            candidatesCausingChange = candidatesCausingChange.Add(candidate);
                        }
                        else
                        {
                            candidatesToRemove = candidatesToRemove.Add(candidate);
                        }
                    }
                }
            }
        }

        var change = BoardStateChange.ForCandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        var hinter = candidatesCausingChange.Any()
            ? new LockedCandidatesPointingHinter(Orientation, candidatesCausingChange)
            : (IChangeHinter)NoHints.Instance;
        return new ChangeDescription(change, hinter, this);
    }
}

internal record LockedCandidatesPointingHinter(IOrientation Orientation, ImmutableHashSet<Candidate> Causers) : IChangeHinter
{
    public IEnumerable<ChangeHint> GetHints()
    {
        yield return new ChangeHint("Use Locked Candidates Pointing");
        yield return new ChangeHint($"Look for candidates that appear in only a single {Orientation.PrimaryDimensionName} in Box {Causers.First().Position.Box + 1}");
        yield return new ChangeHint($"The candidate value is {Causers.First().CandidateValue}");
        yield return new ChangeHint($"These are the locked candidates", BoardStateChange.ForCandidatesCausingChange(Causers));
    }
}
