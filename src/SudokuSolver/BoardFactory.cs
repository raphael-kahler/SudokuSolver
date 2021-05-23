using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public static class BoardFactory
    {
        private readonly static ImmutableHashSet<int> AllCandidates = Enumerable.Range(1, 9).ToImmutableHashSet();

        public static BoardState EmptyBoard()
        {
            var cells = ImmutableList<Cell>.Empty;

            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 9; ++col)
                {
                    cells = cells.Add(EmptyCell(row, col));
                }
            }

            return new BoardState(cells);
        }

        public static Cell EmptyCell(int row, int col) => new Cell(new Position(row, col), null, AllCandidates);
    }
}