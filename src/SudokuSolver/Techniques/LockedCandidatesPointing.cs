using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques
{
    public class LockedCandidatesPointing : ISolverTechnique
    {
        private IOrientation Orientation { get; }
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public string Description =>
            $"In one box all candidates of a number are in the same {Orientation.PrimaryDimensionName}. " +
            $"Remove candidates from other boxes of that {Orientation.PrimaryDimensionName}.";

        internal LockedCandidatesPointing(IOrientation orientation)
        {
            Orientation = orientation ?? throw new System.ArgumentNullException(nameof(orientation));
        }

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            for (int box = 0; box < 9; ++box)
            {
                var cells = board.Box(box);
                for (int value = 1; value <= 9; ++value)
                {
                    var change = GetChangeForValue(board, cells, value);
                    if (change.HasEffect)
                    {
                        return new ChangeDescription(change, NoHints.Instance, this);
                    }
                }
            }
            return NoChangeDescription.Instance;
        }

        private IBoardStateChange GetChangeForValue(BoardState board, IEnumerable<Cell> cells, int value)
        {
            var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            var cellsForCandidate = cells.Where(c => c.Candidates.Contains(value)).ToList();
            if (cellsForCandidate.Any())
            {
                var dimValue = Orientation.PrimaryDimension(cellsForCandidate.First().Position);
                if (cellsForCandidate.All(c => Orientation.PrimaryDimension(c.Position) == dimValue))
                {
                    foreach (var cell in Orientation.CellsForPrimaryDimension(board, dimValue))
                    {
                        if (cell.Candidates.Contains(value))
                        {
                            var candidate = new Candidate(cell.Position, value);
                            if (cellsForCandidate.Contains(cell))
                            {
                                candidatesCausingChange = candidatesCausingChange.Add(candidate);
                            }
                            else
                            {
                                candidatesToRemove = candidatesToRemove.Add(candidate);
                            }
                        }
                    }
                }
            }

            return BoardStateChange.CandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
        }

        public static LockedCandidatesPointing Row() => new LockedCandidatesPointing(RowOrientation.Instance);
        public static LockedCandidatesPointing Column() => new LockedCandidatesPointing(ColumnOrientation.Instance);
        public static IEnumerable<LockedCandidatesPointing> AllDirections() => new List<LockedCandidatesPointing> { Row(), Column() };
    }
}