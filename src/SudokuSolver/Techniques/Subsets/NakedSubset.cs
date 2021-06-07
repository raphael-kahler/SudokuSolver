using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Subsets
{
    internal class NakedSubset : SubsetTechniqueBase
    {
        public override DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;
        public override string Description =>
            $"{Size} cells in a {CellCollector.CollectionName} have only the same {Size} candidates. " +
            $"Those candidates can be removed from all other cells in the {CellCollector.CollectionName}.";

        internal NakedSubset(int size, ICellCollector cellCollector) : base(size, cellCollector)
        { }

        protected override string TechniqueName() => $"Naked {SizeName()}";

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) =>
            CellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
            var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

            var cellsWithTwoCandidates = cells.Where(c => c.Candidates.Count == Size).ToList();
            for (int i = 0; i < cellsWithTwoCandidates.Count - 1; ++i)
            {
                var candidates = cellsWithTwoCandidates[i].Candidates;
                var numMatchingCells = cellsWithTwoCandidates.Skip(i + 1).Count(c => c.Candidates.SetEquals(candidates));
                if (numMatchingCells == Size - 1)
                {
                    foreach (var cell in cells)
                    {
                        if (cell.Candidates.SetEquals(candidates))
                        {
                            foreach (var candidate in candidates)
                            {
                                candidatesCausingChange = candidatesCausingChange.Add(new Candidate(cell.Position, candidate));
                            }
                        }
                        else
                        {
                            foreach (var candidate in candidates)
                            {
                                if (cell.Candidates.Contains(candidate))
                                {
                                    candidatesToRemove = candidatesToRemove.Add(new Candidate(cell.Position, candidate));
                                }
                            }
                        }
                    }
                }
            }

            return CreateChangeDescription(candidatesCausingChange, candidatesToRemove);
        }
    }
}