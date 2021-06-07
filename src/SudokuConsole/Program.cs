using CommandLine;
using SudokuSolver;
using SudokuSolver.Rules;
using SudokuSolver.Techniques;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuConsole
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "The input file containing the Sudoku puzzle to solve.")]
        public string InputFile { get; set; }

        [Option('n', "nextStep", Required = false, HelpText = "If provided, solve only the next step for the Sudoku. If not provided, solve the Sudoku completely.")]
        public bool NextStep { get; set; }
    }

    internal class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => SolveSudoku(options),
                    _ => 1
                );
        }

        private static int SolveSudoku(Options options)
        {
            using var boardPrinter = new ConsoleBoardPrinter();
            var board = InputReader.CreateBoardFromFile(options.InputFile);
            var rules = new StandardSudokuRules();
            ISolver solver = CreateSolver();
            try
            {
                Console.WriteLine("Initial board.");
                boardPrinter.PrintLarge(board);
                if (options.NextStep)
                {
                    SolveNextStep(board, boardPrinter, solver, rules);
                }
                else
                {
                    SolveCompletely(board, boardPrinter, solver, rules);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static ISolver CreateSolver()
        {
            return new ChainedSolver()
                .WithSolver(new Solver().WithTechnique(Subsets.NakedSingle()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(EliminationByValue.AllDirections()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(Subsets.HiddenSingleRow()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(Subsets.HiddenSingleColumn()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(Subsets.HiddenSingleBox()).GlobChanges())
                .WithSolver(new Solver()
                    .WithTechnique(LockedSubsets.LockedCandidatesPointing.AllDirections())
                    .WithTechnique(LockedSubsets.LockedCandidateClaiming.AllDirections())
                    .WithTechnique(Subsets.NakedPairs())
                    .WithTechnique(Subsets.HiddenPairs())
                    .WithTechnique(Subsets.NakedTriples())
                    .WithTechnique(Subsets.HiddenTriples())
                    .WithTechnique(Subsets.NakedQuads())
                    .WithTechnique(Subsets.HiddenQuads())
                    .WithTechnique(FishTechnique.XWing())
                    .WithTechnique(WingTechnique.XyWing())
                    .WithTechnique(FishTechnique.Swordfish())
                    .WithTechnique(FishTechnique.Jellyfish())
                    .WithTechnique(WingTechnique.XyzWing())
                    .WithTechnique(WingTechnique.WxyzWing()));
        }

        private static IChangeDescription SolveNextStep(BoardState board, ConsoleBoardPrinter boardPrinter, ISolver solver, ISudokuRules rules)
        {
            if (board.IsComplete)
            {
                Console.WriteLine("Sudoku is already solved.");
                return NoChangeDescription.Instance;
            }
            var changeDescription = solver.GetNextChange(board);
            if (!changeDescription.Change.HasEffect)
            {
                Console.WriteLine("Couldn't find any more steps for solving the Sudoku :(");
                return changeDescription;
            }

            var newBoard = board.ApplyChange(changeDescription.Change);
            Console.WriteLine($"{changeDescription.FoundBy.DifficultyLevel} step: {changeDescription.FoundBy.Description}");
            boardPrinter.PrintLarge(newBoard, changeDescription.Change);

            if (!rules.BoardIsValid(newBoard))
            {
                Console.WriteLine("Oh no! Got into an invalid state :(");
                return changeDescription;
            }

            if (newBoard.IsComplete)
            {
                Console.WriteLine("Solved :)");
                return changeDescription;
            }

            return changeDescription;
        }

        private static void SolveCompletely(BoardState board, ConsoleBoardPrinter boardPrinter, ISolver solver, ISudokuRules rules)
        {
            var changeHistory = new List<IChangeDescription>();
            while (true)
            {
                var changeDescription = SolveNextStep(board, boardPrinter, solver, rules);
                board = board.ApplyChange(changeDescription.Change);
                changeHistory.Add(changeDescription);
                if (!changeDescription.Change.HasEffect)
                {
                    break;
                }
                if (board.IsComplete)
                {
                    PrintSummary(changeHistory);
                    break;
                }
            }
        }

        private static void PrintSummary(IReadOnlyList<IChangeDescription> changeHistory)
        {
            Console.WriteLine();
            Console.WriteLine("Applied techniques in order:");
            foreach (var change in changeHistory)
            {
                Console.WriteLine($"{change.FoundBy.DifficultyLevel} step: {change.FoundBy.Description}");
            }

            Console.WriteLine();
            Console.WriteLine("Technique uses per difficulty level:");
            foreach (var difficultyLevel in Enum.GetValues<DifficultyLevel>().Skip(1))
            {
                var usageCount = changeHistory.Count(c => c.FoundBy.DifficultyLevel == difficultyLevel);
                Console.WriteLine($"{difficultyLevel,10}: {usageCount,5}");
            }
        }
    }
}
