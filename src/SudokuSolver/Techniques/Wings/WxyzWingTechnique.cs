using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.Helpers.Sets;

namespace SudokuSolver.Techniques.Wings
{
    public class WxyzWingTechnique : ISolverTechnique
    {
        public string Description => "WXYZ-Wing";
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Expert;

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            var cellsWithTwoCandidates = board.Cells.Where(c => c.Candidates.Count == 2);

            var allCollections = CellCollections.GetRows(board).Concat(CellCollections.GetColumns(board)).Concat(CellCollections.GetBoxes(board));
            var almostLockedSets = allCollections.SelectMany(c => SetFinder.FindAlmostLockedSets(c, size: 3));

            foreach (var almostLockedSet in almostLockedSets)
            {
                foreach (var cell in cellsWithTwoCandidates)
                {
                    var maybeWing = almostLockedSet.FormsWxyzWingWith(cell);
                    if (maybeWing.Is)
                    {
                        var wxyzWing = maybeWing.Value;
                        var candidatesToRemove = FindCandidatesToRemove(board, wxyzWing);
                        if (candidatesToRemove.Any())
                        {
                            var candidatesCausingChange = wxyzWing.GetDefiningCandidates().ToImmutableHashSet();
                            var change = BoardStateChange.ForCandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                            return new ChangeDescription(change, NoHints.Instance, this);
                        }
                    }
                }
            }

            return NoChangeDescription.Instance;
        }

        private IEnumerable<Cell> AffectedCells(BoardState board, WxyzWing wing) => wing.CollectionType switch
        {
            BoxCollectionType => board.Row(wing.WzCell.Position.Row).Concat(board.Column(wing.WzCell.Position.Col)),
            RowCollectionType => board.Box(wing.WzCell.Position.Box),
            ColumnCollectionType => board.Box(wing.WzCell.Position.Box),
            _ => Enumerable.Empty<Cell>()
        };

        private ImmutableHashSet<Candidate> FindCandidatesToRemove(BoardState board, WxyzWing wing)
        {
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            foreach (var cell in AffectedCells(board, wing))
            {
                if (wing.AppliesTo(cell))
                {
                    candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, wing.ZValue));
                }
            }

            return candidatesToRemove;
        }
    }

    internal class WxyzWingTechniqueHinter : IChangeHinter
    {
        private readonly WxyzWing wxyzWing;

        public WxyzWingTechniqueHinter(WxyzWing xyzWing) =>
            this.wxyzWing = xyzWing ?? throw new System.ArgumentNullException(nameof(xyzWing));

        public IEnumerable<ChangeHint> GetHints()
        {
            yield return new ChangeHint($"Use WXYZ-Wing technique");
            yield return new ChangeHint($"The Z value is {this.wxyzWing.ZValue}");
            yield return new ChangeHint($"This is the W cell", BoardStateChange.ForCandidatesCausingChange(
                this.wxyzWing.WzCell.GetCandidatesWithPosition().ToImmutableHashSet()));
            yield return new ChangeHint($"This is the WXYZ-Wing", BoardStateChange.ForCandidatesCausingChange(
                this.wxyzWing.GetDefiningCandidates().ToImmutableHashSet()));
        }
    }
}