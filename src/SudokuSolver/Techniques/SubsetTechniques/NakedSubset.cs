using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.SubsetTechniques
{
    internal class NakedSubset : CollectionCandidateRemover
    {
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public override string Description =>
            $"{size} cells in a {cellCollector.CollectionName} have only the same {size} candidates. " +
            $"Those candidates can be removed from all other cells in the {cellCollector.CollectionName}.";

        internal NakedSubset(int size, ICellCollector cellCollector)
        {
            this.size = size;
            this.cellCollector = cellCollector;
        }

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            cellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var candidatesCausingChange = new HashSet<Candidate>();
            var candidatesToRemove = new HashSet<Candidate>();

            var cellsWithTwoCandidates = cells.Where(c => c.Candidates.Count == size).ToList();
            for (int i = 0; i < cellsWithTwoCandidates.Count - 1; ++i)
            {
                var candidates = cellsWithTwoCandidates[i].Candidates;
                var numMatchingCells = cellsWithTwoCandidates.Skip(i + 1).Count(c => c.Candidates.SetEquals(candidates));
                if (numMatchingCells == size - 1)
                {
                    foreach (var cell in cells)
                    {
                        if (cell.Candidates.SetEquals(candidates))
                        {
                            foreach (var candidate in candidates)
                            {
                                candidatesCausingChange.Add(new Candidate(cell.Position, candidate));
                            }
                        }
                        else
                        {
                            foreach (var candidate in candidates)
                            {
                                if (cell.Candidates.Contains(candidate))
                                {
                                    candidatesToRemove.Add(new Candidate(cell.Position, candidate));
                                }
                            }
                        }
                    }
                }
            }

            var change = BoardStateChange.ForCandidatesRemovingCandidates(
                candidatesCausingChange.ToImmutableHashSet(),
                candidatesToRemove.ToImmutableHashSet());
            var hinter = new SubsetHinter(TechniqueName(), cellCollector, candidatesCausingChange.First().Position);

            return new ChangeDescription(change, NoHints.Instance, this);
        }

        private string TechniqueName() => $"Naked {SizeName()}";
        private string SizeName() => size switch
        {
            1 => "Single",
            2 => "Pair",
            3 => "Triple",
            4 => "Quad",
            _ => "Collection"
        };
    }
}