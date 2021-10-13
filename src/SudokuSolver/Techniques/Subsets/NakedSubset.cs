using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Subsets;

internal class NakedSubset : SubsetTechniqueBase
{
    public override DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
    public override string Description =>
        $"{Size} cells in a {CellCollector.CollectionName} have only the same {Size} candidates. " +
        $"Those candidates can be removed from all other cells in the {CellCollector.CollectionName}.";

    internal NakedSubset(int size, ICellCollector cellCollector) : base(size, cellCollector)
    { }

    protected override string TechniqueName() => $"Naked {SizeName()}";

    protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
        CellCollector.GetCollections(board);

    protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
    {

        var cellsWithCandidates = cells.Where(c => c.Candidates.Count > 0 && c.Candidates.Count <= Size).ToList();

        if (cellsWithCandidates.Count < Size)
        {
            return NoChangeDescription.Instance;
        }

        var combinations = CollectionPermutator.Permutate(cellsWithCandidates.Count, Size);
        foreach (var combination in combinations)
        {
            var candidateCount = combination.SelectMany(idx => cellsWithCandidates[idx].Candidates).Distinct().Count();
            if (candidateCount == Size)
            {
                var candidatesCausingChange = combination.SelectMany(idx => cellsWithCandidates[idx].GetCandidatesWithPosition()).ToImmutableHashSet();
                var candidates = candidatesCausingChange.Select(c => c.CandidateValue);

                var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;
                foreach (var cell in cells)
                {
                    if (!candidatesCausingChange.Any(c => c.Position == cell.Position))
                    {
                        foreach (var candidate in cell.Candidates)
                        {
                            if (candidates.Contains(candidate))
                            {
                                candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, candidate));
                            }
                        }
                    }
                }

                if (candidatesToRemove.Any())
                {
                    return CreateChangeDescription(candidatesCausingChange, candidatesToRemove);
                }
            }
        }

        return NoChangeDescription.Instance;
    }
}
