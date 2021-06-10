using System.Collections.Generic;
using SudokuSolver;
using SudokuSolver.Techniques;

namespace Site.Solver
{
    public interface ITechnique
    {
        string Name { get; }

        bool IsCollection { get; }
    }

    public class TechniqueCollection : ITechnique
    {
        public bool IsCollection => true;
        public string Name { get; }
        public IReadOnlyList<ITechnique> TechniqueCollections { get; }

        public TechniqueCollection(string name, IReadOnlyList<ITechnique> techniqueCollections)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            TechniqueCollections = techniqueCollections ?? throw new System.ArgumentNullException(nameof(techniqueCollections));
        }

        // public void FindChangeFor(BoardState board)
        // {
        //     foreach (var collection in TechniqueCollections)
        //     {
        //         collection.FindChangeFor(board);
        //     }
        // }
    }

    public class TechniqueWrapper : ITechnique
    {
        public bool IsCollection => false;
        public string Name { get; }
        public ISolverTechnique Technique { get; }

        public TechniqueWrapper(string name, ISolverTechnique technique)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            Technique = technique ?? throw new System.ArgumentNullException(nameof(technique));
        }
    }
}