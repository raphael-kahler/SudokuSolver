namespace SudokuSolver.Techniques.Helpers;

internal class CellCollections
{
    public static IEnumerable<IEnumerable<Cell>> GetRows(BoardState board) =>
        Enumerable.Range(0, 9).Select(row => board.Row(row));

    public static IEnumerable<IEnumerable<Cell>> GetColumns(BoardState board) =>
        Enumerable.Range(0, 9).Select(col => board.Column(col));

    public static IEnumerable<IEnumerable<Cell>> GetBoxes(BoardState board) =>
        Enumerable.Range(0, 9).Select(box => board.Box(box));

    public static IEnumerable<IEnumerable<Cell>> GetAllCollections(BoardState board) =>
        GetRows(board).Concat(GetColumns(board)).Concat(GetBoxes(board));
}
