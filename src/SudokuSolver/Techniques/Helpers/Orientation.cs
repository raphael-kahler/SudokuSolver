using System.Collections.Generic;

namespace SudokuSolver.Techniques.Helpers
{
    internal interface IOrientation
    {
        string PrimaryDimensionName { get; }
        int PrimaryDimension(Position position);
        int SecondaryDimension(Position position);
        Position ToPosition(int primary, int secondary);
        IEnumerable<Cell> CellsForPrimaryDimension(BoardState board, int primaryDimensionValue);
        IEnumerable<Cell> CellsForSecondaryDimension(BoardState board, int secondaryDimensionValue);
    }

    internal class RowOrientation : IOrientation
    {
        public string PrimaryDimensionName => "Row";
        public IEnumerable<Cell> CellsForPrimaryDimension(BoardState board, int primaryDimensionValue) => board.Row(primaryDimensionValue);
        public IEnumerable<Cell> CellsForSecondaryDimension(BoardState board, int secondaryDimensionValue) => board.Column(secondaryDimensionValue);
        public int PrimaryDimension(Position position) => position.Row;
        public int SecondaryDimension(Position position) => position.Col;
        public Position ToPosition(int primary, int secondary) => new Position(primary, secondary);

        private RowOrientation() { }
        public static RowOrientation Instance { get; } = new RowOrientation();
    }

    internal class ColumnOrientation : IOrientation
    {
        public string PrimaryDimensionName => "Column";
        public IEnumerable<Cell> CellsForPrimaryDimension(BoardState board, int primaryDimensionValue) => board.Column(primaryDimensionValue);
        public IEnumerable<Cell> CellsForSecondaryDimension(BoardState board, int secondaryDimensionValue) => board.Row(secondaryDimensionValue);
        public int PrimaryDimension(Position position) => position.Col;
        public int SecondaryDimension(Position position) => position.Row;
        public Position ToPosition(int primary, int secondary) => new Position(secondary, primary);

        private ColumnOrientation() { }
        public static ColumnOrientation Instance { get; } = new ColumnOrientation();
    }
}