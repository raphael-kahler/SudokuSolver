using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Techniques
{
    public class SingleCandidateInCell : ISolverTechnique
    {
        public DifficultyLevel DifficultyLevel => DifficultyLevel.Trivial;
        public string Description => "A cell has only one candidate.";

        public IBoardStateChange GetPossibleBoardStateChange(BoardState board)
        {
            var match = board.Cells.FirstOrDefault(c => !c.Value.HasValue && c.Candidates.Count == 1);
            return match != null
                ? ConstructChange(match)
                : new SudokuSolver.BoardStateNoChange();
        }

        private BoardStateChangeSetNumber ConstructChange(Cell match)
        {
            var candidate = match.Candidates.Single();
            var changeExplanation = ChangeDescription.ValueSetter(
                candidatesCausingChange: ImmutableHashSet<Candidate>.Empty.Add(new Candidate(match.Position, candidate)),
                valueAffected: new Cell(match.Position, candidate, ImmutableHashSet<int>.Empty));

            return new BoardStateChangeSetNumber(match.Position, candidate, this, changeExplanation);
        }
    }
}