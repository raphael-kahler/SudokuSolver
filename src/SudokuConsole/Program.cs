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
        [Option('i', "input", Required = true, HelpText = "The input file containing the sudoku puzzle to solve.")]
        public string InputFile { get; set; }

        [Option('n', "nextStep", Required = false, HelpText = "If provided, solve only the next step for the sudoku. If not provided, solve the sudoku completely.")]
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
            Solver solver = CreateSolver();
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

        private static Solver CreateSolver() => new Solver()
            .WithTechnique(NakedSubset.NakedSingle())
            .WithTechnique(EliminationByValue.AllDirections())
            .WithTechnique(HiddenSubset.HiddenSingles())
            .WithTechnique(LockedCandidatesPointing.AllDirections())
            .WithTechnique(LockedCandidateClaiming.AllDirections())
            .WithTechnique(NakedSubset.NakedPairs())
            .WithTechnique(HiddenSubset.HiddenPairs())
            .WithTechnique(NakedSubset.NakedTriples())
            .WithTechnique(HiddenSubset.HiddenTriples())
            .WithTechnique(NakedSubset.NakedQuads())
            .WithTechnique(HiddenSubset.HiddenQuads())
            .WithTechnique(FishTechnique.XWing())
            .WithTechnique(WingTechnique.XyWing())
            .WithTechnique(FishTechnique.Swordfish())
            .WithTechnique(FishTechnique.Jellyfish());

        private static IBoardStateChange SolveNextStep(BoardState board, ConsoleBoardPrinter boardPrinter, Solver solver, ISudokuRules rules)
        {
            if (board.IsComplete)
            {
                Console.WriteLine("Sudoku is already solved.");
                return new BoardStateNoChange();
            }
            var change = solver.GetNextChange(board);
            if (!change.CausesChange)
            {
                Console.WriteLine("Couldn't find any more steps for solving the sudoku :(");
            }

            var newBoard = board.ApplyChange(change);
            Console.WriteLine($"{change.FoundBy.DifficultyLevel} step: {change.FoundBy.Description}");
            boardPrinter.PrintLarge(newBoard, change.Description);

            if (!rules.BoardIsValid(newBoard))
            {
                Console.WriteLine("Oh no! Got into an invalid state :(");
                return change;
            }

            if (newBoard.IsComplete)
            {
                Console.WriteLine("Solved :)");
                return change;
            }

            return change;
        }

        private static void SolveCompletely(BoardState board, ConsoleBoardPrinter boardPrinter, Solver solver, ISudokuRules rules)
        {
            var changeHistory = new List<IBoardStateChange>();
            while (true)
            {
                var change = SolveNextStep(board, boardPrinter, solver, rules);
                board = board.ApplyChange(change);
                changeHistory.Add(change);
                if (!change.CausesChange)
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

        private static void PrintSummary(IReadOnlyList<IBoardStateChange> changeHistory)
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
