using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques.Subsets
{
    internal class NakedSingle : ISolverTechnique
    {
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Trivial;
        public string Description => "A cell has only one candidate.";

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            var match = board.Cells.FirstOrDefault(c => !c.Value.HasValue && c.Candidates.Count == 1);
            return match != null
                ? ConstructChange(match)
                : NoChangeDescription.Instance;
        }

        private ChangeDescription ConstructChange(Cell match)
        {
            var candidate = match.Candidates.Single();
            var change = BoardStateChange.ForCandidatesSettingValues(
                candidatesCausingChange: ImmutableHashSet<Candidate>.Empty.Add(new Candidate(match.Position, candidate)),
                valueAffected: new Cell(match.Position, candidate, ImmutableHashSet<int>.Empty));

            return new ChangeDescription(change, NoHints.Instance, this);
        }
    }
}