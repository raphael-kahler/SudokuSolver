﻿using CommandLine;
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
                .WithSolver(new Solver().WithTechnique(NakedSubset.NakedSingle()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(EliminationByValue.AllDirections()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(HiddenSubset.HiddenSingleRow()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(HiddenSubset.HiddenSingleColumn()).GlobChanges())
                .WithSolver(new Solver().WithTechnique(HiddenSubset.HiddenSingleBox()).GlobChanges())
                .WithSolver(new Solver()
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
                    .WithTechnique(FishTechnique.Jellyfish())
                    .WithTechnique(WingTechnique.XyzWing())
                    .WithTechnique(WingTechnique.WxyzWing()));
        }

        private static IBoardStateChange SolveNextStep(BoardState board, ConsoleBoardPrinter boardPrinter, ISolver solver, ISudokuRules rules)
        {
            if (board.IsComplete)
            {
                Console.WriteLine("Sudoku is already solved.");
                return BoardStateNoChange.Instance;
            }
            var change = solver.GetNextChange(board);
            if (!change.CausesChange)
            {
                Console.WriteLine("Couldn't find any more steps for solving the Sudoku :(");
                return change;
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

        private static void SolveCompletely(BoardState board, ConsoleBoardPrinter boardPrinter, ISolver solver, ISudokuRules rules)
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
