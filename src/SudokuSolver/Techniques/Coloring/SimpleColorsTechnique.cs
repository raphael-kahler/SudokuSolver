using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SudokuSolver.Techniques.Helpers;

namespace SudokuSolver.Techniques.Coloring
{
    internal class ColorGraph
    {
        public HashSet<ColorNode> Nodes { get; } = new HashSet<ColorNode>();
        public bool AllNodesVisited() => Nodes.All(n => n.Visited);

        public ColorNode Add(Position position)
        {
            var match = Nodes.FirstOrDefault(n => n.Position == position);
            if (match != null)
            {
                return match;
            }
            var newNode = new ColorNode(position);
            Nodes.Add(newNode);
            return newNode;
        }

        public void ClearColors()
        {
            foreach (var node in Nodes)
            {
                node.ColorValue = null;
            }
        }
    }

    internal class ColorNode
    {
        public Position Position { get; init; }
        public int? ColorValue { get; set; }
        public bool Visited { get; set; }
        public HashSet<ColorNode> Neighbors { get; }

        public ColorNode(Position position, int? colorValue = null)
        {
            Position = position;
            ColorValue = colorValue;
            Neighbors = new HashSet<ColorNode>();
        }

        public void Link(ColorNode other)
        {
            Neighbors.Add(other);
            other.Neighbors.Add(this);
        }

        public void Merge(ColorNode other)
        {
            foreach (var neighbor in other.Neighbors)
            {
                Neighbors.Add(neighbor);
            }
        }

        public override bool Equals(object obj) =>
            obj is ColorNode node &&
            EqualityComparer<Position>.Default.Equals(Position, node.Position);

        public override int GetHashCode() => HashCode.Combine(Position);
    }

    internal class SimpleColorsTechnique : ISolverTechnique
    {
        public string Description => "Simple coloring";

        public DifficultyLevel DifficultyLevel => DifficultyLevel.Medium;

        public IChangeDescription GetPossibleBoardStateChange(BoardState board)
        {
            for (int value = 1; value <= 9; ++value)
            {
                var change = GetChangeForValue(board, value);
                if (change.Change.HasEffect)
                {
                    return change;
                }
            }

            return NoChangeDescription.Instance;
        }

        private IChangeDescription GetChangeForValue(BoardState board, int value)
        {
            var cellsOfValue = board.Cells.Where(c => c.Candidates.Contains(value));
            var graph = FindConjugatePairs(board, value);

            ImmutableHashSet<Candidate> candidatesAffected = ImmutableHashSet<Candidate>.Empty;

            while (!graph.AllNodesVisited())
            {
                graph.ClearColors();
                var node = graph.Nodes.FirstOrDefault(n => !n.Visited);
                VisitNode(node, colorValue: 0);
                foreach (var cell in cellsOfValue)
                {
                    var connectingColorCount = graph.Nodes
                        .Where(n => n.ColorValue.HasValue && n.Position.ConnectsToDistinct(cell.Position))
                        .Select(n => n.ColorValue)
                        .Distinct()
                        .Count();

                    if (connectingColorCount > 1)
                    {
                        candidatesAffected = candidatesAffected.Add(new Candidate(cell.Position, value));
                    }
                }
                if (candidatesAffected.Any())
                {
                    var causers = graph.Nodes.Where(n => n.ColorValue.HasValue)
                        .GroupBy(n => n.ColorValue.Value)
                        .Select(group => group.Select(n => new Candidate(n.Position, value)).ToImmutableHashSet().Except(candidatesAffected))
                        .ToImmutableList<IImmutableSet<Candidate>>();
                    var change = BoardStateChange.ForCandidateGroupsRemovingCandidates(causers, candidatesAffected);
                    return new ChangeDescription(change, NoHints.Instance, this);
                }
            }

            return NoChangeDescription.Instance;
        }

        private static void VisitNode(ColorNode node, int colorValue)
        {
            node.Visited = true;
            node.ColorValue = colorValue;
            foreach (var neighbor in node.Neighbors.Where(n => !n.Visited))
            {
                VisitNode(neighbor, 1 - colorValue);
            }
        }

        private static ColorGraph FindConjugatePairs(BoardState board, int value)
        {
            var graph = new ColorGraph();
            foreach (var cells in CellCollections.GetAllCollections(board))
            {
                AddConjugatePairToGraph(value, graph, cells);
            }
            return graph;
        }

        private static void AddConjugatePairToGraph(int value, ColorGraph graph, IEnumerable<Cell> cells)
        {
            if (cells.Count(c => c.Candidates.Contains(value)) == 2)
            {
                var node1 = graph.Add(cells.Where(c => c.Candidates.Contains(value)).First().Position);
                var node2 = graph.Add(cells.Where(c => c.Candidates.Contains(value)).Skip(1).First().Position);
                node1.Link(node2);
            }
        }
    }
}