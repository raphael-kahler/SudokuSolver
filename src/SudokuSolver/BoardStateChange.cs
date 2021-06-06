using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques;

namespace SudokuSolver
{
    public interface IBoardStateChange
    {
        bool CausesChange { get; }
        ISolverTechniqueDescription FoundBy { get; }
        IChangeHinter ChangeHinter { get; }
        IChangeDescription Description { get; }

        BoardState ApplyTo(BoardState board);
    }

    public class BoardStateNoChange : IBoardStateChange
    {
        public bool CausesChange => false;
        public ISolverTechniqueDescription FoundBy => NotFound.Instance;
        public IChangeHinter ChangeHinter => NoHints.Instance;
        public IChangeDescription Description => NoChangeDescription.Instance;

        public BoardState ApplyTo(BoardState board) => board with { LastChange = this };

        private BoardStateNoChange() { }
        public static BoardStateNoChange Instance { get; } = new BoardStateNoChange();
    }

    public record BoardStateChangeClearCell(Position Position)
        : IBoardStateChange
    {
        public bool CausesChange => true;
        public IChangeHinter ChangeHinter => NoHints.Instance;
        public ISolverTechniqueDescription FoundBy => NotFound.Instance;
        public IChangeDescription Description => NoChangeDescription.Instance;
        public BoardState ApplyTo(BoardState board) => new BoardState(
            cells: board.Cells.Replace(
                oldValue: board.Cell(Position),
                newValue: Cell.Empty(Position)),
            lastChange: this);
    }

    public record BoardStateChangeSetNumber(Position Position, int Value, ISolverTechniqueDescription FoundBy, IChangeDescription Description)
        : IBoardStateChange
    {
        public bool CausesChange => true;
        public IChangeHinter ChangeHinter => NoHints.Instance;

        public BoardStateChangeSetNumber(Position position, int value)
            : this(position, value, NotFound.Instance, NoChangeDescription.Instance)
        { }

        public BoardState ApplyTo(BoardState board) => new BoardState(
            cells: board.Cells.Replace(
                oldValue: board.Cell(Position),
                newValue: new Cell(Position, Value, ImmutableHashSet<int>.Empty)),
            lastChange: this);
    }

    public record BoardStateChangeCandidateRemoval(IReadOnlyCollection<Candidate> CandidatesToRemove, ISolverTechniqueDescription FoundBy, IChangeDescription Description, IChangeHinter ChangeHinter)
        : IBoardStateChange
    {
        public bool CausesChange => true;

        public BoardStateChangeCandidateRemoval(IReadOnlyList<Candidate> candidatesToRemove)
            : this(candidatesToRemove, NotFound.Instance, NoChangeDescription.Instance, NoHints.Instance)
        { }

        public BoardState ApplyTo(BoardState board)
        {
            var cells = board.Cells;
            foreach (var c in CandidatesToRemove)
            {
                var oldCell = cells.Single(cell => cell.Position == c.Position);
                var newCell = oldCell with { Candidates = oldCell.Candidates.Remove(c.CandidateValue) };
                cells = cells.Replace(oldCell, newCell);
            }
            return new BoardState(cells, this);
        }
    }

    public record BoardStateChangeCombination(IReadOnlyCollection<IBoardStateChange> Changes)
        : IBoardStateChange
    {
        public bool CausesChange => Changes.Any(c => c.CausesChange);
        public IChangeHinter ChangeHinter => NoHints.Instance;

        public ISolverTechniqueDescription FoundBy =>
            new CombinedTechnique(Changes.Select(c => c.FoundBy).ToList());

        public IChangeDescription Description =>
            new ChangeDescriptionCombination(Changes.Select(c => c.Description).ToList());

        public BoardState ApplyTo(BoardState board)
        {
            foreach (var change in Changes)
            {
                board = board.ApplyChange(change);
            }
            return board;
        }
    }
}
