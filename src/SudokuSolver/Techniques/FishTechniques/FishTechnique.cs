using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.FishTechniques;

internal class FishTechnique : ISolverTechnique
{
    public string Description => $"{Size}-{Orientation.PrimaryDimensionName} Fish.";
    public DifficultyLevel DifficultyLevel => Size < 3 ? DifficultyLevel.Advanced : DifficultyLevel.Expert;
    private int Size { get; }
    private IOrientation Orientation { get; }

    internal FishTechnique(int size, IOrientation oritentation)
    {
        Size = size;
        Orientation = oritentation ?? throw new System.ArgumentNullException(nameof(oritentation));
    }

    public IChangeDescription GetPossibleBoardStateChange(BoardState board)
    {
        for (int value = 1; value <= 9; ++value)
        {
            var changeDescription = GetChangeForValue(board, value);
            if (changeDescription.Change.HasEffect)
            {
                return changeDescription;
            }
        }

        return NoChangeDescription.Instance;
    }

    private IChangeDescription GetChangeForValue(BoardState board, int value)
    {
        var cellCollections = GetCellCollections(board)
            .Select(cells => cells.Where(c => c.Candidates.Contains(value)).Select(c => c.Position).ToList())
            .ToList();

        var cells = new List<Position>[Size];
        foreach (var permutation in CollectionPermutator.Permutate(Size, cellCollections.Count))
        {
            for (int i = 0; i < Size; ++i)
            {
                cells[i] = cellCollections[permutation[i]];
            }
            var fish = GetFishFinder().GetFishType(cells);
            var removals = fish.DetermineCandidateRemovals(board, value).ToImmutableHashSet();
            if (removals.Any())
            {
                var causers = fish.DefiningCandidates(value).ToImmutableHashSet();
                var change = BoardStateChange.ForCandidatesRemovingCandidates(causers, removals);
                var hinter = new FishTechniqueHinter(fish, Size, value, Orientation);
                return new ChangeDescription(change, hinter, this);
            }
        }

        return NoChangeDescription.Instance;
    }

    private IList<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
        Enumerable.Range(0, 9)
            .Select(value => Orientation.CellsForPrimaryDimension(board, value))
            .ToList();

    private IFishFinder GetFishFinder() => Size == 2 ? new TwoFishFinder(Orientation) : new LargeFishFinder(Size, Orientation);
}

internal record FishTechniqueHinter(IFish Fish, int Size, int CandidateValue, IOrientation Orientation) : IChangeHinter
{
    public IEnumerable<ChangeHint> GetHints()
    {
        yield return new ChangeHint($"Use a {FishNamer.GetFishName(Size)} ({Size}-Fish) technique");
        yield return new ChangeHint($"The fish is for candidates of value {CandidateValue}");
        yield return new ChangeHint($"The fish is {Orientation.PrimaryDimensionName} based");
        yield return new ChangeHint($"It is a {Fish.FishType} {FishNamer.GetFishName(Size)}");
        yield return new ChangeHint($"This is the fish", BoardStateChange.ForCandidatesCausingChange(Fish.DefiningCandidates(CandidateValue).ToImmutableHashSet()));
    }
}
