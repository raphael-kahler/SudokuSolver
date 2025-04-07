using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Subsets;

internal class HiddenSubset : SubsetTechniqueBase
{
    private record CellsForCandidate(int CandidateValue, IReadOnlyList<Cell> Cells);

    public override DifficultyLevel DifficultyLevel => Size == 1 ? DifficultyLevel.Easy : DifficultyLevel.Medium;

    public override string Description =>
        $"{Size} number{(Size > 1 ? "s are" : " is")} the candidate of only {Size} " +
        $"cell{(Size > 1 ? "s" : string.Empty)} in a {CellCollector.CollectionName}. All other candidates of the cell{(Size > 1 ? "s" : string.Empty)} can be removed.";

    internal HiddenSubset(int size, ICellCollector cellCollector) : base(size, cellCollector)
    { }

    protected override string TechniqueName() => $"Hidden {SizeName()}";

    protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) => CellCollector.GetCollections(board);

    protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
    {
        var cellsForCandidates = Enumerable.Range(1, 9)
            .Select(value => new CellsForCandidate(value, cells.Where(c => c.Candidates.Contains(value)).ToList()))
            .Where(c => c.Cells.Count > 0 && c.Cells.Count <= Size)
            .ToList();

        if (cellsForCandidates.Count < Size)
        {
            return NoChangeDescription.Instance;
        }

        foreach (var combination in CollectionPermutator.Permutate(Size, cellsForCandidates.Count))
        {
            var cellsForCombination = combination.SelectMany(idx => cellsForCandidates[idx].Cells.Select(c => c.Position)).ToHashSet();
            if (cellsForCombination.Count == Size)
            {
                var candidatesInCombination = combination.Select(idx => cellsForCandidates[idx].CandidateValue);
                var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
                var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

                foreach (var cell in cells.Where(c => cellsForCombination.Contains(c.Position)))
                {
                    foreach (var candidate in cell.Candidates)
                    {
                        var candidateToRecord = new Candidate(cell.Position, candidate);
                        if (candidatesInCombination.Contains(candidate))
                        {
                            candidatesCausingChange = candidatesCausingChange.Add(candidateToRecord);
                        }
                        else
                        {
                            candidatesToRemove = candidatesToRemove.Add(candidateToRecord);
                        }
                    }
                }

                return CreateChangeDescription(candidatesCausingChange, candidatesToRemove);
            }
        };

        return NoChangeDescription.Instance;
    }
}
