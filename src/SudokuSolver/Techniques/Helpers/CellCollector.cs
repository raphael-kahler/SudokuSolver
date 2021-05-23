using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    internal interface ICellCollector
    {
        string CollectionName { get; }
        IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board);
        IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx);
    }

    internal class RowCellCollector : ICellCollector
    {
        private RowCellCollector() { }
        public static RowCellCollector Instance { get; } = new RowCellCollector();

        public string CollectionName => "row";

        public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetRows(board);
        public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Row(collectionIdx);
    }

    internal class ColumnCellCollector : ICellCollector
    {
        private ColumnCellCollector() { }
        public static ColumnCellCollector Instance { get; } = new ColumnCellCollector();
        public string CollectionName => "column";
        public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetColumns(board);
        public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Column(collectionIdx);
    }
 
    internal class BoxCellCollector : ICellCollector
    {
        private BoxCellCollector() { }
        public static BoxCellCollector Instance { get; } = new BoxCellCollector();
        public string CollectionName => "box";
        public IEnumerable<IEnumerable<Cell>> GetCollections(BoardState board) => CellCollections.GetBoxes(board);
        public IEnumerable<Cell> GetCollection(BoardState board, int collectionIdx) => board.Box(collectionIdx);
    }
}