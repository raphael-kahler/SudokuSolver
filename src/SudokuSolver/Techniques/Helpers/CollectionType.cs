namespace SudokuSolver.Techniques.Helpers;

internal interface ICollectionType
{
    int Value(Position position);
}

internal class RowCollectionType : ICollectionType
{
    public int Value(Position position) => position.Row;
    private RowCollectionType() { }
    public static RowCollectionType Instance { get; } = new RowCollectionType();

}

internal class ColumnCollectionType : ICollectionType
{
    public int Value(Position position) => position.Col;
    private ColumnCollectionType() { }
    public static ColumnCollectionType Instance { get; } = new ColumnCollectionType();

}

internal class BoxCollectionType : ICollectionType
{
    public int Value(Position position) => position.Box;
    private BoxCollectionType() { }
    public static BoxCollectionType Instance { get; } = new BoxCollectionType();

}
