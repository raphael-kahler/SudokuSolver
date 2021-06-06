using System;
using System.IO;
using System.Linq;
using SudokuSolver;
using SudokuSolver.Techniques;

namespace SudokuConsole
{
    public static class InputReader
    {
        public static BoardState CreateBoardFromFile(string inputFilePath)
        {
            var lines = File.ReadAllLines(inputFilePath);

            if (lines[0].Length == 9)
            {
                return ParseSimpleBoard(lines);
            }
            else
            {
                return ParseBoardWithCandidates(lines);
            }
        }

        private static BoardState ParseSimpleBoard(string[] lines)
        {
            var board = BoardFactory.CandidateBoard();
            for (int row = 0; row < 9; ++ row)
            {
                var line = lines[row];
                for (int col = 0; col < 9; ++col)
                {
                    var inputValue = line[col];
                    if (int.TryParse(inputValue.ToString(), out int value))
                    {
                        board = board.ApplyChange(new BoardStateChangeSetNumber(new Position(row, col), value));
                    }
                }
            }
            return board;
        }

        private static BoardState ParseBoardWithCandidates(string[] lines)
        {
            var board = BoardFactory.CandidateBoard();
            for (int row = 0; row < 9; ++ row)
            {
                var line = lines[row];
                var columns = line.Split("-;, ".ToCharArray(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                for (int col = 0; col < 9; ++col)
                {
                    var position = new Position(row, col);
                    var cellValues = columns[col].Select(value => int.Parse(value.ToString())).ToList();
                    if (cellValues.Count == 1)
                    {
                        board = board.ApplyChange(new BoardStateChangeSetNumber(position, cellValues.Single()));
                    }
                    else
                    {
                        var removals = Enumerable.Range(1, 9)
                            .Where(value => !cellValues.Contains(value))
                            .Select(value => new Candidate(position, value))
                            .ToList();

                        board = board.ApplyChange(new BoardStateChangeCandidateRemoval(removals));
                    }
                }
            }
            return board;
        }
    }
}