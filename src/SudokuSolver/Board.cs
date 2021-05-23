using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver
{
    public record Position(int Row, int Col)
    {
        public int Box { get; } = (Row / 3) * 3 + (Col / 3);
        public bool Is(int row, int col) => Row == row && Col == col;
        public bool ConnectsTo(Position other) => Row == other.Row || Col == other.Col || Box == other.Box;
    }

    public record Candidate(Position Position, int CandidateValue);
    public record Cell(Position Position, int? Value, IImmutableSet<int> Candidates);

    public record BoardState
    {
        public IImmutableList<Cell> Cells { get; }
        public IBoardStateChange LastChange { get; init; }

        public BoardState(IImmutableList<Cell> cells) : this(cells, new BoardStateNoChange())
        { }

        public BoardState(IImmutableList<Cell> cells, IBoardStateChange lastChange)
        {
            this.Cells = cells ?? throw new ArgumentNullException(nameof(cells));
            this.LastChange = lastChange ?? throw new ArgumentNullException(nameof(lastChange));

            if (cells.Count != 81)
            {
                throw new ArgumentException("A proper board required 81 cells.", nameof(cells));
            }

            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 9; ++col)
                {
                    Position position = new (row, col);
                    if (!cells.Any(c => c.Position == position))
                    {
                        throw new ArgumentException($"List contains no cell for position {position}.", nameof(cells));
                    }
                }
            }
        }

        public bool IsComplete => Cells.All(c => c.Value.HasValue);
        public Cell Cell(Position position) => Cells.Single(c => c.Position == position);
        public Cell Cell(int row, int col) => Cells.Single(c => c.Position.Is(row, col));
        public IEnumerable<Cell> Row(int row) => Cells.Where(c => c.Position.Row == row);
        public IEnumerable<Cell> Column(int col) => Cells.Where(c => c.Position.Col == col);
        public IEnumerable<Cell> Box(int boxIdx)
        {
            for (int rowIdx = 0; rowIdx < 3; ++rowIdx)
            {
                var row = (boxIdx / 3) * 3 + rowIdx;
                for (int colIdx = 0; colIdx < 3; ++colIdx)
                {
                    var col = (boxIdx % 3) * 3 + colIdx;
                    yield return Cell(row, col);
                }
            }
        }

        public BoardState ApplyChange(IBoardStateChange change) => change.ApplyTo(this);
    }
}
