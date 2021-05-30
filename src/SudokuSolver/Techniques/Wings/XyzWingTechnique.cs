using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques.Wings
{
    public class XyzWingTechnique : ISolverTechnique
    {
        public string Description => "XYZ-Wing";
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Expert;

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var cells = board.Cells.Where(c => c.Candidates.Count == 3);

            foreach (var xyzCell in cells)
            {
                var xyzCandidates = xyzCell.Candidates;
                foreach (var xzCell in board.Box(xyzCell.Position.Box))
                {
                    var xzCandidates = xzCell.Candidates;
                    if (xzCandidates.Count == 2
                        && xzCandidates.SetEquals(xzCandidates.Intersect(xyzCandidates)))
                    {
                        var yCandidate = xyzCandidates.Except(xzCandidates).Single();
                        foreach (var yzCell in board.Row(xyzCell.Position.Row).Concat(board.Column(xyzCell.Position.Col)))
                        {
                            if (yzCell.Position.Box != xyzCell.Position.Box
                                && yzCell.Candidates.Count == 2
                                && yzCell.Candidates.Contains(yCandidate))
                            {
                                foreach (var candidate in xzCandidates)
                                {
                                    if (yzCell.Candidates.Contains(candidate))
                                    {
                                        // xyz wing found
                                        var zCandidate = xzCandidates.Single(c => c != candidate);
                                        var xyzWing = new XyzWing(xyzCell, xzCell, yzCell, zCandidate);
                                        var candidatesToRemove = FindCandidatesToRemove(board, xyzWing);
                                        if (candidatesToRemove.Any())
                                        {
                                            var candidatesCausingChange = xyzWing.GetDefiningCandidates().ToImmutableHashSet();
                                            var change = ChangeDescription.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                                            return new BoardStateChangeCandidateRemoval(candidatesToRemove, this, change);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new BoardStateNoChange();
        }

        private ImmutableHashSet<Candidate> FindCandidatesToRemove(BoardState board, XyzWing xyzWing)
        {
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            foreach (var cell in board.Box(xyzWing.Pivot.Position.Box))
            {
                if (xyzWing.AppliesTo(cell))
                {
                    candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, xyzWing.ZValue));
                }
            }

            return candidatesToRemove;
        }
    }
}