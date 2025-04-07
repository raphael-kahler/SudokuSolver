using SudokuSolver.Functional;
using SudokuSolver.Techniques.Wings;

namespace SudokuSolver.Techniques.Helpers.Sets;

internal record AlmostLockedSet
{
    public IReadOnlyList<Cell> SetCells { get; }
    public ICollectionType CollectionType { get; }
    public int Size => SetCells.Count;
    public IEnumerable<int> Candidates => SetCells.SelectMany(c => c.Candidates).Distinct();

    public AlmostLockedSet(IReadOnlyList<Cell> setCells)
    {
        SetCells = setCells ?? throw new ArgumentNullException(nameof(setCells));
        var firstPosition = SetCells.First().Position;
        if (SetCells.All(c => c.Position.Row == firstPosition.Row))
        {
            CollectionType = RowCollectionType.Instance;
        }
        else if (SetCells.All(c => c.Position.Col == firstPosition.Col))
        {
            CollectionType = ColumnCollectionType.Instance;
        }
        else if (SetCells.All(c => c.Position.Box == firstPosition.Box))
        {
            CollectionType = BoxCollectionType.Instance;
        }
        else
        {
            throw new ArgumentException($"Set cells have to all be in either the same row, column, or box. But the provided cells were not: {string.Join(", ", setCells)}.", nameof(setCells));
        }
    }

    public virtual bool Equals(AlmostLockedSet other) => SetCells.SequenceEqual(other.SetCells);
    public override int GetHashCode() => HashCode.Combine(SetCells);

    public Maybe<WxyzWing> FormsWxyzWingWith(Cell cell)
    {
        if (CollectionType.Value(cell.Position) == CollectionType.Value(SetCells.First().Position))
        {
            return Maybe<WxyzWing>.None;
        }
        var connectingCells = SetCells.Where(c => c.Position.ConnectsTo(cell.Position));
        if (!connectingCells.Any())
        {
            return Maybe<WxyzWing>.None;
        }
        var sharedCandidates = cell.Candidates;
        sharedCandidates = cell.Candidates.Intersect(connectingCells.SelectMany(c => c.Candidates));
        if (!sharedCandidates.SetEquals(cell.Candidates))
        {
            return Maybe<WxyzWing>.None;
        }
        var nonConnectingCells = SetCells.Where(c => !c.Position.ConnectsTo(cell.Position));
        var overlapCandidates = sharedCandidates.Intersect(nonConnectingCells.SelectMany(c => c.Candidates));
        if (overlapCandidates.Count() != 1)
        {
            return Maybe<WxyzWing>.None;
        }
        var zValue = overlapCandidates.Single();
        return new WxyzWing(this, cell, zValue, CollectionType);

    }
}
