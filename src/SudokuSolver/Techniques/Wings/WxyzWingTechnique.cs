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

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
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
                            var change = ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                            return new BoardStateChangeCandidateRemoval(candidatesToRemove, this, change);
                        }
                    }
                }
            }

            return BoardStateNoChange.Instance;
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
}