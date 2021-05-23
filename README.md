# Sudoku Solver

A solver for Sudoku puzzles that uses common [solving techniques](http://sudopedia.enjoysudoku.com/Solving_Technique.html).
The solver shows for every step which technique was used, so you can follow along how the puzzle was solved.
You can use this for learning techniques or to get a hint for the sudoku you are stuck on.

## Usage

Install [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet) or newer to be able to build the projects.

``` sh
cd src/SudokuConsole
dotnet build
```

To solve a sudoku completely, just provide the path to a sudoku input file.

``` sh
./bin/Debug/net5.0/SudokuConsole.exe -i ../../inputs/sudoku-basic1.txt
```

To solve only the next step of an in-progress sudoku, you can use the `--nextStep` flag.

``` sh
./bin/Debug/net5.0/SudokuConsole.exe -i ../../inputs/withCandidates/sudoku-evil1.txt --nextStep
```
