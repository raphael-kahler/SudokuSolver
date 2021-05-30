using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;
using SudokuSolver.Techniques.Helpers.Sets;

namespace SudokuSolver.Techniques.Wings
{
    internal interface IWing
    {
        bool AppliesTo(Cell cell);
        IEnumerable<Candidate> GetDefiningCandidates();
    }

    internal class XyWing : IWing
    {
        public Cell Pivot { get; }
        public IImmutableList<Cell> Wings { get; }
        public int WingValue { get; }

        public XyWing(Cell cell1, Cell cell2, Cell cell3)
        {
            if (!cell2.Position.ConnectsTo(cell3.Position))
            {
                Pivot = cell1;
                Wings = ImmutableList<Cell>.Empty.Add(cell2).Add(cell3);
            }
            else if (!cell1.Position.ConnectsTo(cell3.Position))
            {
                Pivot = cell2;
                Wings = ImmutableList<Cell>.Empty.Add(cell1).Add(cell3);
            }
            else
            {
                Pivot = cell3;
                Wings = ImmutableList<Cell>.Empty.Add(cell1).Add(cell2);
            }
            WingValue = Wings[0].Candidates.Single(c => !Pivot.Candidates.Contains(c));
        }

        public bool AppliesTo(Cell cell) =>
            cell.Candidates.Contains(WingValue) && Wings.All(w => w.Position != cell.Position && w.Position.ConnectsTo(cell.Position));

        public IEnumerable<Candidate> GetDefiningCandidates() =>
            Wings.SelectMany(w => w.GetCandidatesWithPosition())
                .Concat(Pivot.GetCandidatesWithPosition());
    }

    internal class XyzWing : IWing
    {
        public Cell Pivot { get; }
        public IImmutableList<Cell> Wings { get; }
        public int ZValue { get; }

        public XyzWing(Cell xyzCell, Cell xzCell, Cell yzCell, int zValue)
        {
            Pivot = xyzCell;
            Wings = ImmutableList<Cell>.Empty.Add(xzCell).Add(yzCell);
            ZValue = zValue;
        }

        public bool AppliesTo(Cell cell) =>
            cell.Candidates.Contains(ZValue)
            && Pivot.Position != cell.Position
            && Pivot.Position.ConnectsTo(cell.Position)
            && Wings.All(w => w.Position != cell.Position && w.Position.ConnectsTo(cell.Position));

        public IEnumerable<Candidate> GetDefiningCandidates() =>
            Wings.SelectMany(w => w.GetCandidatesWithPosition())
                .Concat(Pivot.GetCandidatesWithPosition());
    }

    internal record WxyzWing(AlmostLockedSet Set, Cell WzCell, int ZValue, ICollectionType CollectionType)
        : IWing
    {
        public bool AppliesTo(Cell cell) =>
            cell.Candidates.Contains(ZValue)
            && WzCell.Position.ConnectsToDistinct(cell.Position)
            && Set.SetCells.All(c => c.Position.ConnectsToDistinct(cell.Position));

        public IEnumerable<Candidate> GetDefiningCandidates() =>
            Set.SetCells.SelectMany(w => w.GetCandidatesWithPosition())
                .Concat(WzCell.GetCandidatesWithPosition());
    }
}