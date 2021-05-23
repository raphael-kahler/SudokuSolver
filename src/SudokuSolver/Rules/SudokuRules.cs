using System.Linq;

namespace SudokuSolver.Rules
{
    public interface ISudokuRules
    {
        bool BoardIsValid(BoardState board);
    }

    public class StandardSudokuRules : ISudokuRules
    {
        public bool BoardIsValid(BoardState board) =>
            ValidateOneNumberPerRow(board)
            && ValidateOneNumberPerColumn(board)
            && ValidateOneNumberPerBox(board);

        private bool ValidateOneNumberPerRow(BoardState board)
        {
            for (int row = 0; row < 9; ++row)
            {
                var rowCells = board.Row(row);
                for (int value = 1; value <= 9; ++value)
                {
                    var count = rowCells.Count(cell => cell.Value == value);
                    if (count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateOneNumberPerColumn(BoardState board)
        {
            for (int column = 0; column < 9; ++column)
            {
                var columnCells = board.Column(column);
                for (int value = 1; value <= 9; ++value)
                {
                    var count = columnCells.Count(cell => cell.Value == value);
                    if (count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateOneNumberPerBox(BoardState board)
        {
            for (int box = 0; box < 9; ++box)
            {
                var boxCells = board.Box(box);
                for (int value = 1; value <= 9; ++value)
                {
                    var count = boxCells.Count(cell => cell.Value == value);
                    if (count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}