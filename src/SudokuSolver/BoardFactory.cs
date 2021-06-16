using System;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Functional;

namespace SudokuSolver
{
    public static class BoardFactory
    {
        private readonly static ImmutableHashSet<int> AllCandidates = Enumerable.Range(1, 9).ToImmutableHashSet();

        public static BoardState CandidateBoard() =>
            CreateBoard((Position position) => new Cell(position, null, AllCandidates));

        public static BoardState EmptyBoard() =>
            CreateBoard((Position position) => Cell.Empty(position));

        private static BoardState CreateBoard(Func<Position, Cell> cellConstructor)
        {
            var cells = ImmutableList<Cell>.Empty;

            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 9; ++col)
                {
                    cells = cells.Add(cellConstructor(new Position(row, col)));
                }
            }

            return new BoardState(cells);
        }

        public static Maybe<BoardState> CreateFromSudokuString(string sudokuString)
        {
            if (sudokuString.Length != 81)
            {
                return Maybe<BoardState>.None;
            }
            var board = BoardFactory.CandidateBoard();
            for (int row = 0; row < 9; ++ row)
            {
                for (int col = 0; col < 9; ++col)
                {
                    var idx = row * 9 + col;
                    var inputValue = sudokuString[idx];
                    if (int.TryParse(inputValue.ToString(), out int value))
                    {
                        if (value > 0)
                        {
                            board = board.ApplyChange(BoardStateChange.SetCell(new Position(row, col), value));
                        }
                    }
                    else
                    {
                        return Maybe<BoardState>.None;
                    }
                }
            }
            return board;
        }
    }
}