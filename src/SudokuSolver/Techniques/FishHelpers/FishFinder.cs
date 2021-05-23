using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.FishHelpers
{
    internal interface IFishFinder
    {
        IFish GetFishType(IList<IList<Position>> positions);
    }

    internal class LargeFishFinder : IFishFinder
    {
        private readonly int size;
        private readonly IOrientation orientation;

        public LargeFishFinder(int size, IOrientation orientation)
        {
            this.size = size;
            this.orientation = orientation;
        }

        public IFish GetFishType(IList<IList<Position>> positions)
        {
            if (!ValidFishSize(positions))
            {
                return NoFish.Instance;
            }

            var positionBySecondaryDim = positions.SelectMany(p => p).GroupBy(p => orientation.SecondaryDimension(p)).ToDictionary(kv => kv.Key, kv => kv.Select(p => p).ToList());

            var definingFishPositions = positionBySecondaryDim.Where(kv => kv.Value.Count > 1);
            var extraFishPositions = positionBySecondaryDim.Where(kv => kv.Value.Count == 1);
            if (definingFishPositions.Count() == size - 1)
            {
                if (extraFishPositions.Count() == 2)
                {
                    // special case where two single positions remain. Use one as position and the other as fin
                    var finPos = extraFishPositions.First().Value.Single();
                    var finLogicalSecondaryDim = extraFishPositions.Skip(1).First().Key;
                    if (BoxForPositionValue(orientation.SecondaryDimension(finPos)) != BoxForPositionValue(finLogicalSecondaryDim))
                    {
                        return NoFish.Instance;
                    }

                    return new FinnedFish(
                        positions.SelectMany(p => p).Where(p => p != finPos).Select(p => new FishCorner(p)).ToImmutableList(),
                        new FishFin(orientation.ToPosition(orientation.PrimaryDimension(finPos), finLogicalSecondaryDim), ImmutableHashSet<Position>.Empty.Add(finPos)),
                        orientation);
                }
                else if (extraFishPositions.Count() == 3)
                {
                    // special case where three single positions remain. The two in the same box+row are the fin, the other is the position.
                    var extras = extraFishPositions.SelectMany(kv => kv.Value).ToList();
                    var finExtras = ImmutableHashSet<Position>.Empty;
                    var finSecondaryDim = 0;
                    if (SameFinBox(extras[0], extras[1]))
                    {
                        if (orientation.PrimaryDimension(extras[0]) != orientation.PrimaryDimension(extras[2]))
                        {
                            finExtras = finExtras.Add(extras[0]).Add(extras[1]);
                            finSecondaryDim = orientation.SecondaryDimension(extras[2]);
                        }
                    }
                    else if (SameFinBox(extras[0], extras[2]))
                    {
                        if (orientation.PrimaryDimension(extras[0]) != orientation.PrimaryDimension(extras[1]))
                        {
                            finExtras = finExtras.Add(extras[0]).Add(extras[2]);
                            finSecondaryDim = orientation.SecondaryDimension(extras[1]);
                        }
                    }
                    else if (SameFinBox(extras[1], extras[2]))
                    {
                        if (orientation.PrimaryDimension(extras[0]) != orientation.PrimaryDimension(extras[1]))
                        {
                            finExtras = finExtras.Add(extras[1]).Add(extras[2]);
                            finSecondaryDim = orientation.SecondaryDimension(extras[0]);
                        }
                    }

                    if (!finExtras.Any()
                        || BoxForPositionValue(orientation.SecondaryDimension(finExtras.First())) != BoxForPositionValue(finSecondaryDim))
                    {
                        return NoFish.Instance;
                    }

                    try
                    {
                        var nonFinPositions = positions.SelectMany(p => p).Where(p => !finExtras.Contains(p)).Select(p => new FishCorner(p)).ToImmutableList();
                        var logicalFinPosition = orientation.ToPosition(orientation.PrimaryDimension(finExtras.First()), finSecondaryDim);

                        return new FinnedFish(nonFinPositions, new FishFin(logicalFinPosition, finExtras), orientation);
                    }
                    catch (System.Exception)
                    {
                        System.Console.WriteLine(string.Join(", ", extras));
                        throw;
                    }
                }
                else
                {
                    return NoFish.Instance;
                }
            }
            if (definingFishPositions.Count() != size)
            {
                return NoFish.Instance;
            }
            if (extraFishPositions.Count() > 2)
            {
                return NoFish.Instance;
            }

            var fishPositions = definingFishPositions.SelectMany(kv => kv.Value);

            if (extraFishPositions.Count() == 0)
            {
                return new RegularFish(fishPositions.Select(p => new FishCorner(p)).ToImmutableList(), orientation);
            }

            var finPositions = extraFishPositions.SelectMany(kv => kv.Value).ToImmutableHashSet();

            return CreateFinnedFish(definingFishPositions, fishPositions, finPositions);
        }

        private IFish CreateFinnedFish(IEnumerable<KeyValuePair<int, List<Position>>> definingFishPositions, IEnumerable<Position> fishPositions, ImmutableHashSet<Position> finPositions)
        {
            var finPrimaryDimension = orientation.PrimaryDimension(finPositions.First());
            var finSecondaryBox = BoxForPositionValue(orientation.SecondaryDimension(finPositions.First()));
            foreach (var finPosition in finPositions.Skip(1))
            {
                var primaryPosition = orientation.PrimaryDimension(finPosition);
                var secondaryBox = BoxForPositionValue(orientation.SecondaryDimension(finPosition));
                if (primaryPosition != finPrimaryDimension || secondaryBox != finSecondaryBox)
                {
                    return NoFish.Instance;
                }
            }
            var matchingFishPositions = definingFishPositions.Where(kv => BoxForPositionValue(kv.Key) == finSecondaryBox);
            if (!matchingFishPositions.Any())
            {
                return NoFish.Instance;
            }

            var finSecondaryDimension = matchingFishPositions.First().Key;
            var logicalFinPosition = orientation.ToPosition(finPrimaryDimension, finSecondaryDimension);
            if (fishPositions.Contains(logicalFinPosition))
            {
                finPositions = finPositions.Add(logicalFinPosition);
            }

            var fin = new FishFin(logicalFinPosition, finPositions);
            return new FinnedFish(fishPositions.Select(p => new FishCorner(p)).ToImmutableList(), fin, orientation);
        }

        private bool ValidFishSize(IList<IList<Position>> positions)
        {
            if (positions.Count != this.size)
            {
                return false;
            }
            if (positions.Any(p => p.Count < 2 || p.Count > this.size + 2))
            {
                return false;
            }
            if (positions.Count(p => p.Count > this.size) > 1)
            {
                return false;
            }
            return true;
        }

        private int BoxForPositionValue(int positionValue) => positionValue / 3;

        private bool SameFinBox(Position position1, Position position2) =>
            orientation.PrimaryDimension(position1) == orientation.PrimaryDimension(position2) && position1.Box == position2.Box;
    }

    internal class TwoFishFinder : IFishFinder
    {
        private readonly IOrientation orientation;

        public TwoFishFinder(IOrientation orientation) => this.orientation = orientation;

        public IFish GetFishType(IList<IList<Position>> positions)
        {
            if (positions.Count != 2)
            {
                return NoFish.Instance;
            }

            var collection1 = positions[0];
            var collection2 = positions[1];

            if (!ValidFishSize(collection1.Count, collection2.Count))
            {
                return NoFish.Instance;
            }

            var finSet = collection1.Count != 2 ? collection1 : collection2; // fin set has 2 or 3 entries
            var nonFinSet = collection1.Count == 2 ? collection1 : collection2; // non-fin set has 2 entries

            foreach (var nonFinSetPos in nonFinSet)
            {
                var secondary1 = orientation.SecondaryDimension(nonFinSetPos);
                foreach (var finSetPos in finSet)
                {
                    var secondary2 = orientation.SecondaryDimension(finSetPos);
                    if (secondary1 == secondary2)
                    {
                        // have three parts of the fish
                        var lastPrimary = orientation.PrimaryDimension(finSetPos);
                        var lastSecondary = orientation.SecondaryDimension(nonFinSet.Single(p => orientation.SecondaryDimension(p) != secondary1));
                        var lastPosition = orientation.ToPosition(lastPrimary, lastSecondary);

                        var remainingFinSetPositions = finSet.Where(p => p != finSetPos);
                        if (remainingFinSetPositions.All(p => p == lastPosition))
                        {
                            // x-wing
                            return new RegularFish(
                                ImmutableList<FishCorner>.Empty
                                    .Add(new FishCorner(nonFinSetPos))
                                    .Add(new FishCorner(finSetPos))
                                    .Add(new FishCorner(nonFinSet.Single(p => orientation.SecondaryDimension(p) != secondary1)))
                                    .Add(new FishCorner(lastPosition)),
                                orientation);
                        }
                        else if (remainingFinSetPositions.All(p => p.Box == lastPosition.Box))
                        {
                            // sashimi x-wing
                            return new FinnedFish(
                                ImmutableList<FishCorner>.Empty
                                    .Add(new FishCorner(nonFinSetPos))
                                    .Add(new FishCorner(finSetPos))
                                    .Add(new FishCorner(nonFinSet.Single(p => orientation.SecondaryDimension(p) != secondary1))),
                                new FishFin(lastPosition, remainingFinSetPositions.ToImmutableHashSet()),
                                orientation);
                        }
                    }
                }
            }

            return NoFish.Instance;
        }

        private bool ValidFishSize(int size1, int size2) =>
            size1 == 2 && (size2 == 2 || size2 == 3 || size2 == 4) ||
            size2 == 2 && (size1 == 2 || size1 == 3 || size1 == 4);
    }
}