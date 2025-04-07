namespace SudokuSolver.Rules;

public interface ISudokuRules
{
    bool BoardIsValid(BoardState board);
    bool BoardChangeIsValid(BoardState state, IBoardStateChange change);
}

public class StandardSudokuRules : ISudokuRules
{
    public bool BoardIsValid(BoardState board) =>
        ValidateOneNumberPerRow(board)
        && ValidateOneNumberPerColumn(board)
        && ValidateOneNumberPerBox(board);

    public bool BoardChangeIsValid(BoardState board, IBoardStateChange change)
    {
        foreach (var cellValueChange in change.ValuesAffected)
        {
            if (!CellValueChangeIsValid(board, cellValueChange))
            {
                return false;
            }
        }

        return true;
    }

    private bool CellValueChangeIsValid(BoardState board, Cell cellValueChange)
    {
        var value = cellValueChange.Value;
        var position = cellValueChange.Position;
        var numberAlreadyExists = board.Row(position.Row)
            .Concat(board.Column(position.Col))
            .Concat(board.Box(position.Box))
            .Any(cell => cell.Value == value);

        return !numberAlreadyExists;
    }

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
