namespace SudokuSolver.Techniques.Helpers;

internal interface ICellCollector
{
    string CollectionName { get; }
    ICollectionIndexer Indexer { get; }
    IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board);
    IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx);
}

internal class RowCellCollector : ICellCollector
{
    private RowCellCollector() { }
    public static RowCellCollector Instance { get; } = new RowCellCollector();

    public string CollectionName => "row";
    public ICollectionIndexer Indexer => RowCollectionIndexer.Instance;

    public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetRows(board);
    public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Row(collectionIdx);
}

internal class ColumnCellCollector : ICellCollector
{
    private ColumnCellCollector() { }
    public static ColumnCellCollector Instance { get; } = new ColumnCellCollector();

    public string CollectionName => "column";
    public ICollectionIndexer Indexer => ColumnCollectionIndexer.Instance;

    public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetColumns(board);
    public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Column(collectionIdx);
}

internal class BoxCellCollector : ICellCollector
{
    private BoxCellCollector() { }
    public static BoxCellCollector Instance { get; } = new BoxCellCollector();

    public string CollectionName => "box";
    public ICollectionIndexer Indexer => BoxCollectionIndexer.Instance;

    public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetBoxes(board);
    public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Box(collectionIdx);
}

internal interface ICollectionIndexer
{
    int CollectionIndex(Position position);
}

internal class RowCollectionIndexer : ICollectionIndexer
{
    private RowCollectionIndexer() { }
    public static RowCollectionIndexer Instance { get; } = new RowCollectionIndexer();
    public int CollectionIndex(Position position) => position.Row;
}

internal class ColumnCollectionIndexer : ICollectionIndexer
{
    private ColumnCollectionIndexer() { }
    public static ColumnCollectionIndexer Instance { get; } = new ColumnCollectionIndexer();
    public int CollectionIndex(Position position) => position.Col;
}

internal class BoxCollectionIndexer : ICollectionIndexer
{
    private BoxCollectionIndexer() { }
    public static BoxCollectionIndexer Instance { get; } = new BoxCollectionIndexer();
    public int CollectionIndex(Position position) => position.Box;
}
