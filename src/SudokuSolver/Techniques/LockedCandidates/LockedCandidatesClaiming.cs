using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.LockedCandidates;

internal class LockedCandidatesClaimingTechnique : ISolverTechnique
{
    private readonly ICellCollector cellCollector;
    public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
    public string Description =>
        $"In one {cellCollector.CollectionName} all candidates of a number are in the same box. " +
        $"Remove candidates from other {cellCollector.CollectionName}s of that box.";

    internal LockedCandidatesClaimingTechnique(ICellCollector cellCollector)
    {
        this.cellCollector = cellCollector ?? throw new System.ArgumentNullException(nameof(cellCollector));
    }

    public IChangeDescription GetPossibleBoardStateChange(BoardState board)
    {
        var candidatesToRemove = new List<Candidate>();
        foreach (var cellCollection in cellCollector.GetCollections(board))
        {
            for (int value = 1; value <= 9; ++value)
            {
                var changeDescription = GetChangeForValue(board, cellCollection, value);
                if (changeDescription.Change.HasEffect)
                {
                    return changeDescription;
                }
            }
        }

        return NoChangeDescription.Instance;
    }

    private ChangeDescription GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
    {
        var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
        var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

        var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
        if (cellsForCandidate.Any())
        {
            var boxId = cellsForCandidate.First().Position.Box;
            if (cellsForCandidate.All(c => c.Position.Box == boxId))
            {
                foreach (var boxCell in board.Box(boxId))
                {
                    if (boxCell.Candidates.Contains(value))
                    {
                        Candidate candidate = new Candidate(boxCell.Position, value);
                        if (cellsForCandidate.Contains(boxCell))
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
            ? new LockedCandidatesClaimingHinter(this.cellCollector, candidatesCausingChange)
            : (IChangeHinter)NoHints.Instance;
        return new ChangeDescription(change, hinter, this);
    }
}

internal record LockedCandidatesClaimingHinter(ICellCollector CellCollector, ImmutableHashSet<Candidate> Causers) : IChangeHinter
{
    public IEnumerable<ChangeHint> GetHints()
    {
        yield return new ChangeHint("Use Locked Candidates Claiming");
        yield return new ChangeHint($"Look for candidates that appear in only a single Box of {CellCollector.CollectionName} {CellCollector.Indexer.CollectionIndex(Causers.First().Position) + 1}");
        yield return new ChangeHint($"The candidate value is {Causers.First().CandidateValue}");
        yield return new ChangeHint($"These are the locked candidates", BoardStateChange.ForCandidatesCausingChange(Causers));
    }
}
