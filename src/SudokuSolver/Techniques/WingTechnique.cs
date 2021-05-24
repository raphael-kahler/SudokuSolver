using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques
{
    internal class XyWing
    {
        public Cell Pivot { get; }
        public IImmutableList<Cell> Wings { get; }
        public int WingValue { get; }

        public XyWing(Cell cell1, Cell cell2, Cell cell3)
        {
            if (!cell2.Position.ConnectsTo(cell3.Position))
            {
                Pivot = cell1;
                Wings = ImmutableList<Cell>.Empty.Add(cell2).Add(cell3);
            }
            else if (!cell1.Position.ConnectsTo(cell3.Position))
            {
                Pivot = cell2;
                Wings = ImmutableList<Cell>.Empty.Add(cell1).Add(cell3);
            }
            else
            {
                Pivot = cell3;
                Wings = ImmutableList<Cell>.Empty.Add(cell1).Add(cell2);
            }
            WingValue = Wings[0].Candidates.Single(c => !Pivot.Candidates.Contains(c));
        }

        public bool AppliesTo(Cell cell) =>
            cell.Candidates.Contains(WingValue) && Wings.All(w => w.Position != cell.Position && w.Position.ConnectsTo(cell.Position));

        public IEnumerable<Candidate> GetDefiningCandidates()
        {
            var definingCandidates = ImmutableHashSet<Candidate>.Empty;
            foreach (var wing in Wings)
            {
                foreach (var candidate in wing.Candidates)
                {
                    yield return new Candidate(wing.Position, candidate);
                }
            }
            foreach (var candidate in Pivot.Candidates)
            {
                yield return new Candidate(Pivot.Position, candidate);
            }
        }
    }

    public static class WingTechnique
    {
        public static XyWingTechnique XyWing() => new XyWingTechnique();
    }

    public class XyWingTechnique : ISolverTechnique
    {
        public string Description => "XY-Wing";
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Advanced;

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var cells = board.Cells.Where(c => c.Candidates.Count == 2).ToList();

            for (int i = 0; i < cells.Count - 2; ++i)
            {
                var cell1 = cells[i];
                for (int j = i + 1; j < cells.Count - 1; ++j)
                {
                    var cell2 = cells[j];
                    if (cell1.Position.ConnectsTo(cell2.Position))
                    {
                        var sharedCandidates = cell1.Candidates.Intersect(cell2.Candidates);
                        if (sharedCandidates.Count == 1)
                        {
                            var lastCellCandidates = cell1.Candidates.SymmetricExcept(cell2.Candidates);
                            for (int k = j + 1; k < cells.Count; ++k)
                            {
                                var cell3 = cells[k];
                                bool connects = cell3.Position.ConnectsTo(cell1.Position) || cell3.Position.ConnectsTo(cell2.Position);
                                if (connects && cell3.Candidates.SetEquals(lastCellCandidates))
                                {
                                    var xyWing = new XyWing(cell1, cell2, cell3);
                                    var candidatesToRemove = FindCandidatesToRemove(board, xyWing);
                                    if (candidatesToRemove.Any())
                                    {
                                        var candidatesCausingChange = xyWing.GetDefiningCandidates().ToImmutableHashSet();
                                        var change = ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                                        return new BoardStateChangeCandidateRemoval(candidatesToRemove, this, change);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new BoardStateNoChange();
        }

        private ImmutableHashSet<Candidate> FindCandidatesToRemove(BoardState board, XyWing xyWing)
        {
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            foreach (var cell in board.Cells)
            {
                if (xyWing.AppliesTo(cell))
                {
                    candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, xyWing.WingValue));
                }
            }

            return candidatesToRemove;
        }
    }
}