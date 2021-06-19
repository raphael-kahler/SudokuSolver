using System.Collections.Generic;
using SudokuSolver;

namespace Site.Lib
{
    internal static class SolverFactory
    {
        public static ISolver DefaultSolver() =>
            new ChainedSolver()
                .WithSolver(new Solver().With(Technique.Subsets.NakedSingle()).GlobChanges())
                .WithSolver(new Solver().With(Technique.EliminationByValue.AllDirections()).GlobChanges())
                .WithSolver(new Solver().With(Technique.Subsets.HiddenSingleRow()).GlobChanges())
                .WithSolver(new Solver().With(Technique.Subsets.HiddenSingleColumn()).GlobChanges())
                .WithSolver(new Solver().With(Technique.Subsets.HiddenSingleBox()).GlobChanges())
                .WithSolver(new Solver()
                    .With(Technique.LockedCandidates.Pointing.AllDirections())
                    .With(Technique.LockedCandidates.Claiming.AllDirections())
                    .With(Technique.Subsets.NakedPairs())
                    .With(Technique.Subsets.HiddenPairs())
                    .With(Technique.Subsets.NakedTriples())
                    .With(Technique.Subsets.HiddenTriples())
                    .With(Technique.Subsets.NakedQuads())
                    .With(Technique.Subsets.HiddenQuads())
                    .With(Technique.Colors.SimpleColoring())
                    .With(Technique.Fish.XWing())
                    .With(Technique.Wings.XyWing())
                    .With(Technique.Fish.Swordfish())
                    .With(Technique.Fish.Jellyfish())
                    .With(Technique.Wings.XyzWing())
                    .With(Technique.Wings.WxyzWing()));

        public static TechniqueCollection TechniqueList() =>
            new TechniqueCollection("Techniques", new List<ITechnique>
            {
                new TechniqueCollection("Basic", new List<ITechnique> {
                    new SolverWrapper("Elimination by Value", new Solver().With(Technique.EliminationByValue.AllDirections()).GlobChanges()),
                }),
                new TechniqueCollection("Subsets", new List<ITechnique> {
                    new TechniqueCollection("Naked", new List<ITechnique> {
                        new SolverWrapper("Naked Single", new Solver().With(Technique.Subsets.NakedSingle()).GlobChanges()),
                        new TechniqueCollection("Naked Pair", new List<ITechnique> {
                            new TechniqueWrapper("Naked Pair (Row)", Technique.Subsets.NakedPairRow()),
                            new TechniqueWrapper("Naked Pair (Column)", Technique.Subsets.NakedPairColumn()),
                            new TechniqueWrapper("Naked Pair (Box)", Technique.Subsets.NakedPairBox()),
                        }),
                        new TechniqueCollection("Naked Triple", new List<ITechnique> {
                            new TechniqueWrapper("Naked Triple (Row)", Technique.Subsets.NakedTripleRow()),
                            new TechniqueWrapper("Naked Triple (Column)", Technique.Subsets.NakedTripleColumn()),
                            new TechniqueWrapper("Naked Triple (Box)", Technique.Subsets.NakedTripleBox()),
                        }),
                        new TechniqueCollection("Naked Quad", new List<ITechnique> {
                            new TechniqueWrapper("Naked Quad (Row)", Technique.Subsets.NakedQuadRow()),
                            new TechniqueWrapper("Naked Quad (Column)", Technique.Subsets.NakedQuadColumn()),
                            new TechniqueWrapper("Naked Quad (Box)", Technique.Subsets.NakedQuadBox()),
                        })
                    }),
                    new TechniqueCollection("Hidden", new List<ITechnique> {
                        new TechniqueCollection("Hidden Single", new List<ITechnique> {
                            new TechniqueWrapper("Hidden Single (Row)", Technique.Subsets.HiddenSingleRow()),
                            new TechniqueWrapper("Hidden Single (Column)", Technique.Subsets.HiddenSingleColumn()),
                            new TechniqueWrapper("Hidden Single (Box)", Technique.Subsets.HiddenSingleBox()),
                        }),
                        new TechniqueCollection("Hidden Pair", new List<ITechnique> {
                            new TechniqueWrapper("Hidden Pair (Row)", Technique.Subsets.HiddenPairRow()),
                            new TechniqueWrapper("Hidden Pair (Column)", Technique.Subsets.HiddenPairColumn()),
                            new TechniqueWrapper("Hidden Pair (Box)", Technique.Subsets.HiddenPairBox()),
                        }),
                        new TechniqueCollection("Hidden Triple", new List<ITechnique> {
                            new TechniqueWrapper("Hidden Triple (Row)", Technique.Subsets.HiddenTripleRow()),
                            new TechniqueWrapper("Hidden Triple (Column)", Technique.Subsets.HiddenTripleColumn()),
                            new TechniqueWrapper("Hidden Triple (Box)", Technique.Subsets.HiddenTripleBox()),
                        }),
                        new TechniqueCollection("Hidden Quad", new List<ITechnique> {
                            new TechniqueWrapper("Hidden Quad (Row)", Technique.Subsets.HiddenQuadRow()),
                            new TechniqueWrapper("Hidden Quad (Column)", Technique.Subsets.HiddenQuadColumn()),
                            new TechniqueWrapper("Hidden Quad (Box)", Technique.Subsets.HiddenQuadBox()),
                        })
                    })
                }),
                new TechniqueCollection("Locked Candidates", new List<ITechnique> {
                    new TechniqueCollection("Pointing", new List<ITechnique> {
                        new TechniqueWrapper("Pointing (Row)", Technique.LockedCandidates.Pointing.Row()),
                        new TechniqueWrapper("Pointing (Column)", Technique.LockedCandidates.Pointing.Column()),
                    }),
                    new TechniqueCollection("Claiming", new List<ITechnique> {
                        new TechniqueWrapper("Claiming (Row)", Technique.LockedCandidates.Claiming.Row()),
                        new TechniqueWrapper("Claiming (Column)", Technique.LockedCandidates.Claiming.Column()),
                    }),
                }),
                new TechniqueCollection("Colorings", new List<ITechnique> {
                    new TechniqueWrapper("Simple Coloring", Technique.Colors.SimpleColoring()),
                }),
                new TechniqueCollection("Fish", new List<ITechnique> {
                    new TechniqueCollection("X-Wing (2-Fish)", new List<ITechnique> {
                        new TechniqueWrapper("X-Wing (Row)", Technique.Fish.TwoRow()),
                        new TechniqueWrapper("X-Wing (Column)", Technique.Fish.TwoColumn()),
                    }),
                    new TechniqueCollection("Swordfish (3-Fish)", new List<ITechnique> {
                        new TechniqueWrapper("Swordfish (Row)", Technique.Fish.ThreeRow()),
                        new TechniqueWrapper("Swordfish (Column)", Technique.Fish.ThreeColumn()),
                    }),
                    new TechniqueCollection("Jellyfish (2-Fish)", new List<ITechnique> {
                        new TechniqueWrapper("Jellyfish (Row)", Technique.Fish.FourRow()),
                        new TechniqueWrapper("Jellyfish (Column)", Technique.Fish.FourColumn()),
                    }),
                }),
                new TechniqueCollection("Wings", new List<ITechnique> {
                    new TechniqueWrapper("XY-Wing", Technique.Wings.XyWing()),
                    new TechniqueWrapper("XYZ-Wing", Technique.Wings.XyzWing()),
                    new TechniqueWrapper("WXYZ-Wing", Technique.Wings.WxyzWing()),
                }),
            });
    }
}