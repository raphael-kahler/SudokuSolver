using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.SubsetTechniques
{
    internal class HiddenSubset : CollectionCandidateRemover
    {
        private record CellsForCandidate(int CandidateValue, IReadOnlyList<Cell> Cells);
        private readonly int size;
        private readonly ICellCollector cellCollector;

        public override DifficultyLevel DifficultyLevel => size == 1 ? DifficultyLevel.Easy : DifficultyLevel.Medium;

        public override string Description =>
            $"{size} number{(size > 1 ? "s are" : " is")} the candidate of only {size} " +
            $"cell{(size > 1 ? "s" : string.Empty)} in a {cellCollector.CollectionName}. All other candidates of the cell{(size > 1 ? "s" : string.Empty)} can be removed.";

        internal HiddenSubset(int size, ICellCollector cellCollector)
        {
            this.size = size;
            this.cellCollector = cellCollector;
        }

        protected override IEnumerable<IEnumerable<Cell>> GetCellCollections(BoardState board) => cellCollector.GetCollections(board);

        protected override IChangeDescription FindChange(IEnumerable<Cell> cells)
        {
            var cellsForCandidates = Enumerable.Range(1, 9)
                .Select(value => new CellsForCandidate(value, cells.Where(c => c.Candidates.Contains(value)).ToList()))
                .Where(c => c.Cells.Count > 0 && c.Cells.Count <= size)
                .ToList();

            if (cellsForCandidates.Count < size)
            {
                return NoChangeDescription.Instance;
            }

            foreach (var combination in CollectionPermutator.Permutate(cellsForCandidates.Count, size))
            {
                var cellsForCombination = combination.SelectMany(idx => cellsForCandidates[idx].Cells.Select(c => c.Position)).ToHashSet();
                if (cellsForCombination.Count == size)
                {
                    var candidatesInCombination = combination.Select(idx => cellsForCandidates[idx].CandidateValue);
                    var candidatesCausingChange = ImmutableHashSet<Candidate>.Empty;
                    var candidatesToRemove = ImmutableHashSet<Candidate>.Empty;

                    foreach (var cell in cells.Where(c => cellsForCombination.Contains(c.Position)))
                    {
                        foreach (var candidate in cell.Candidates)
                        {
                            var candidateToRecord = new Candidate(cell.Position, candidate);
                            if (candidatesInCombination.Contains(candidate))
                            {
                                candidatesCausingChange = candidatesCausingChange.Add(candidateToRecord);
                            }
                            else
                            {
                                candidatesToRemove = candidatesToRemove.Add(candidateToRecord);
                            }
                        }
                    }

                    var change = BoardStateChange.ForCandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
                    var hinter = new SubsetHinter(TechniqueName(), cellCollector, candidatesCausingChange.First().Position);
                    return new ChangeDescription(change, hinter, this);
                }
            };

            return NoChangeDescription.Instance;
        }

        private string TechniqueName() => $"Hidden {SizeName()}";
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