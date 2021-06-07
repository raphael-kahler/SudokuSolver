using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.SubsetTechniques
{
    internal abstract class SubsetTechniqueBase : CollectionCandidateRemover
    {
        protected int Size { get; }
        protected ICellCollector CellCollector { get; }

        protected SubsetTechniqueBase(int size, ICellCollector cellCollector)
        {
            Size = size;
            CellCollector = cellCollector;
        }

        protected IChangeDescription CreateChangeDescription(ImmutableHashSet<Candidate> candidatesCausingChange, ImmutableHashSet<Candidate> candidatesToRemove)
        {
            var change = BoardStateChange.ForCandidatesRemovingCandidates(candidatesCausingChange, candidatesToRemove);
            var hinter = candidatesCausingChange.Any()
                ? new SubsetHinter(TechniqueName(), CellCollector, candidatesCausingChange.First().Position)
                : (IChangeHinter)NoHints.Instance;

            return new ChangeDescription(change, hinter, this);
        }

        protected string SizeName() => Size switch
        {
            1 => "Single",
            2 => "Pair",
            3 => "Triple",
            4 => "Quad",
            _ => "Collection"
        };

        protected abstract string TechniqueName();
    }
}