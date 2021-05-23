using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using SudokuSolver;

namespace SudokuConsole
{
    internal record BoardColors(ConsoleColor Background, ConsoleColor Foreground)
    {
        public bool SameAsCurrentConsoleColors() =>
            Console.BackgroundColor == Background && Console.ForegroundColor == Foreground;

        public void ApplyToConsole()
        {
            Console.BackgroundColor = Background;
            Console.ForegroundColor = Foreground;
        }
    }

    internal sealed class ConsoleBoardPrinter : IDisposable
    {
        private static readonly BoardColors ChangeCauseColors = new BoardColors(ConsoleColor.DarkYellow, ConsoleColor.Black);
        private static readonly BoardColors ChangeResultColors = new BoardColors(ConsoleColor.Green, ConsoleColor.Black);
        private static readonly BoardColors DefaultColors = new BoardColors(Console.BackgroundColor, Console.ForegroundColor);

        public ConsoleBoardPrinter()
        {
            Console.CancelKeyPress += OnCancelKeyPress;
        }

        public void Dispose()
        {
            Console.CancelKeyPress -= OnCancelKeyPress;
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (!e.Cancel)
            {
                // reset console colors before terminating application
                Console.ResetColor();
            }
        }

        internal void Print(BoardState board)
        {
            Console.WriteLine("╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗");
            for (int row = 0; row < 9; ++row)
            {
                var builder = new StringBuilder("║ ");
                for (int col = 0; col < 9; ++col)
                {
                    builder.Append(board.Cell(row, col).Value?.ToString() ?? " ");
                    builder.Append((col + 1) % 3 == 0 ? " ║ " : " │ ");
                }
                Console.WriteLine(builder.ToString());

                if (row == 2 || row == 5)
                {
                    Console.WriteLine("╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣");
                }
                else if (row < 8)
                {
                    Console.WriteLine("╟───┼───┼───╫───┼───┼───╫───┼───┼───╢");
                }
            }
            Console.WriteLine("╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝");
        }

        internal void PrintLarge(BoardState board) => PrintLarge(board, NoChangeDescription.Instance);

        internal void PrintLarge(BoardState board, IChangeDescription changeDescription)
        {
            Console.WriteLine("╔═══════╤═══════╤═══════╦═══════╤═══════╤═══════╦═══════╤═══════╤═══════╗");
            for (int row = 0; row < 9; ++row)
            {
                for (int i = 0; i < 3; ++i)
                {
                    PrintLineWithChanges(board, row, i, changeDescription);
                }

                if (row == 2 || row == 5)
                {
                    Console.WriteLine("╠═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣");
                }
                else if (row < 8)
                {
                    Console.WriteLine("╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢");
                }
            }
            Console.WriteLine("╚═══════╧═══════╧═══════╩═══════╧═══════╧═══════╩═══════╧═══════╧═══════╝");
        }

        private void PrintLineWithChanges(BoardState board, int row, int i, IChangeDescription changeDescription)
        {
            var builder = new StringBuilder("║");
            for (int col = 0; col < 9; ++col)
            {
                var cell = board.Cell(row, col);
                if (!changeDescription.RelatedToPosition(cell.Position))
                {
                    for (int j = 1; j <= 3; ++j)
                    {
                        int positionInCell = 3 * i + j;
                        builder.Append(GetLargeCellPaddingString(cell, positionInCell));
                        builder.Append(GetLargeCellString(cell, positionInCell));
                    }
                }
                else
                {
                    for (int j = 1; j <= 3; ++j)
                    {
                        var positionInCell = 3 * i + j;

                        var colors = GetColorsForPositionPadding(cell.Position, positionInCell, changeDescription);
                        ApplyColorChange(builder, colors);
                        builder.Append(GetLargeCellPaddingString(cell, positionInCell));

                        colors = GetColorsForPosition(cell.Position, positionInCell, changeDescription);
                        ApplyColorChange(builder, colors);
                        builder.Append(GetLargeCellString(cell, positionInCell));
                    }
                }
                ApplyColorChange(builder, DefaultColors);
                builder.Append((col + 1) % 3 == 0 ? " ║" : " │");
            }
            Console.WriteLine(builder.ToString());
        }

        private static void ApplyColorChange(StringBuilder builder, BoardColors colors)
        {
            if (!colors.SameAsCurrentConsoleColors())
            {
                Console.Write(builder.ToString());
                builder.Clear();
                colors.ApplyToConsole();
            }
        }

        private BoardColors GetColorsForPosition(Position position, int positionInCell, IChangeDescription changeDescription)
        {
            if (changeDescription.ValuesCausingChange.Any(c => c == position))
            {
                return ChangeCauseColors;
            }
            else if (changeDescription.ValuesAffected.Any(c => c.Position == position))
            {
                return ChangeResultColors;
            }
            else if (changeDescription.CandidatesCausingChange.Any(c => c.Position == position && c.CandidateValue == positionInCell))
            {
                return ChangeCauseColors;
            }
            else if (changeDescription.CandidatesAffected.Any(c => c.Position == position && c.CandidateValue == positionInCell))
            {
                return ChangeResultColors;
            }
            return DefaultColors;
        }

        private BoardColors GetColorsForPositionPadding(Position position, int positionInCell, IChangeDescription changeDescription)
        {
            if (positionInCell % 3 != 1)
            {
                if (changeDescription.ValuesCausingChange.Any(c => c == position))
                {
                    return ChangeCauseColors;
                }
                else if (changeDescription.ValuesAffected.Any(c => c.Position == position))
                {
                    return ChangeResultColors;
                }
            }

            return DefaultColors;
        }

        private string GetLargeCellString(Cell cell, int position) =>
            cell.Value.HasValue
                ? GetCellValueString(position, cell.Value.Value)
                : GetCellCandidateString(position, cell.Candidates);

        private string GetLargeCellPaddingString(Cell cell, int position) =>
            cell.Value.HasValue
                ? GetCellValuePaddingString(position)
                : " ";

        private string GetCellValueString(int position, int value) => position switch
        {
            1 => "┌",
            2 => "─",
            3 => "┐",
            4 => "│",
            6 => "│",
            7 => "└",
            8 => "─",
            9 => "┘",
            5 => $"{value}",
            _ => "░"
        };

        private string GetCellValuePaddingString(int position) => position switch
        {
            2 => "─",
            3 => "─",
            8 => "─",
            9 => "─",
            _ => " ",
        };

        private string GetCellCandidateString(int position, IImmutableSet<int> candidates) =>
            candidates.Contains(position) ? $"{position}" : " ";
    }
}