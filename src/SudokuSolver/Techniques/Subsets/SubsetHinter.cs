using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Subsets;

internal record SubsetHinter(string TechniqueName, ICellCollector CellCollector, Position Position) : IChangeHinter
{
    public IEnumerable<ChangeHint> GetHints()
    {
        yield return new ChangeHint($"Find a {TechniqueName} in a {CellCollector.CollectionName}");
        yield return new ChangeHint($"It is in {CellCollector.CollectionName} {CellCollector.Indexer.CollectionIndex(Position) + 1}");
    }
}
