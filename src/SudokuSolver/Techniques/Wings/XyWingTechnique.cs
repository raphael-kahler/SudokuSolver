using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques.Wings
{
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
                                        return new BoardStateChangeCandidateRemoval(candidatesToRemove, this, change, new XyWingTechniqueHinter(xyWing));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return BoardStateNoChange.Instance;
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

    internal class XyWingTechniqueHinter : IChangeHinter
    {
        private readonly XyWing xyWing;
        public bool HasHints => true;

        public XyWingTechniqueHinter(XyWing xyWing) =>
            this.xyWing = xyWing ?? throw new System.ArgumentNullException(nameof(xyWing));

        public IEnumerable<ChangeHint> GetHints()
        {
            yield return new ChangeHint($"Use XY-Wing technique");
            yield return new ChangeHint($"This is the pivot", ChangeDescription.ForCandidatesCausingChange(
                this.xyWing.Pivot.GetCandidatesWithPosition().ToImmutableHashSet()));
            yield return new ChangeHint($"This is the XY-Wing", ChangeDescription.ForCandidatesCausingChange(
                this.xyWing.GetDefiningCandidates().ToImmutableHashSet()));
        }
    }
}