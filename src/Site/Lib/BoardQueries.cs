using System;
using System.Web;
using Microsoft.AspNetCore.Components;
using SudokuSolver;
using SudokuSolver.Functional;

namespace Site.Lib
{
    internal static class BoardQueries
    {
        public static Maybe<BoardState> GetSudokuBoardFromQuery(NavigationManager navigationManager, bool zerosAreEmpty = false)
        {
            var query = new Uri(navigationManager.Uri).Query;
            var queryParameters = HttpUtility.ParseQueryString(query);
            var puzzleInput = queryParameters["puzzle"];
            if (!string.IsNullOrWhiteSpace(puzzleInput))
            {
                return BoardFactory.CreateFromSudokuString(puzzleInput, zerosAreEmpty);
            }
            return Maybe<BoardState>.None;
        }


    }
}