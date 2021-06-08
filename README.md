# Sudoku Solver

A solver for Sudoku puzzles that uses common [solving techniques](http://sudopedia.enjoysudoku.com/Solving_Technique.html).
The solver shows for every step which technique was used, so you can follow along how the puzzle was solved.
You can use this for learning techniques or to get a hint for the Sudoku you are stuck on.

![Console Output Example](https://i.imgur.com/US62oen.png)

## Techniques

The following techniques are implemented so far. Each technique can individually be added to the solver.

| Technique |  Usage |
| --------- |  ----- |
| **Basic Techniques** | |
| Each value can only appear once per row/column/box. | `Technique.EliminationByValue.AllDirections()` |
| **Subset Techniques** | |
| [Naked Single](http://sudopedia.enjoysudoku.com/Naked_Single.html) | `Technique.Subsets.NakedSingle()` |
| [Naked Pair](http://sudopedia.enjoysudoku.com/Naked_Pair.html) | `Technique.Subsets.NakedPairs()` |
| [Naked Triple](http://sudopedia.enjoysudoku.com/Naked_Triple.html) | `Technique.Subsets.NakedTriples()` |
| [Naked Quad](http://sudopedia.enjoysudoku.com/Naked_Quad.html) | `Technique.Subsets.NakedQuads()` |
| [Hidden Single](http://sudopedia.enjoysudoku.com/Hidden_Single.html) | `Technique.Subsets.HiddenSingles()` |
| [Hidden Pair](http://sudopedia.enjoysudoku.com/Hidden_Pair.html) | `Technique.Subsets.HiddenPairs()` |
| [Hidden Triple](http://sudopedia.enjoysudoku.com/Hidden_Triple.html) | `Technique.Subsets.HiddenTriples()` |
| [Hidden Quad](http://sudopedia.enjoysudoku.com/Hidden_Quad.html) | `Technique.Subsets.HiddenQuads()` |
| **Intersection Techniques** | |
| [Locked Candidates Pointing](http://sudopedia.enjoysudoku.com/Locked_Candidates.html) | `Technique.LockedCandidates.Pointing.AllDirections()` |
| [Locked Candidates Claiming](http://sudopedia.enjoysudoku.com/Locked_Candidates.html) | `Technique.LockedCandidates.Claiming.AllDirections()` |
| **Fish Techniques** | |
| 2-Fish: [X-Wing](http://sudopedia.enjoysudoku.com/X-Wing.html), [Finned X-Wing](http://sudopedia.enjoysudoku.com/Finned_X-Wing.html), [Sashimi X-Wing](http://sudopedia.enjoysudoku.com/Finned_X-Wing.html) | `Technique.Fish.XWing()` |
| 3-Fish: [SwordFish](http://sudopedia.enjoysudoku.com/Swordfish.html), [Finned Swordfish](http://sudopedia.enjoysudoku.com/Finned_Swordfish.html), [Sashimi Swordfish](http://sudopedia.enjoysudoku.com/Sashimi_Swordfish.html) |  `Technique.Fish.Swordfish()` |
| 4-Fish: [Jellyfish](http://sudopedia.enjoysudoku.com/Jellyfish.html), [Finned Jellyfish](http://sudopedia.enjoysudoku.com/Finned_Jellyfish.html), [Sashimi Jellyfish](http://sudopedia.enjoysudoku.com/Sashimi_Jellyfish.html) |  `Technique.Fish.Jellyfish()` |
| **Wing Techniques** | |
| [XY-Wing](http://sudopedia.enjoysudoku.com/XY-Wing.html) | `Technique.Wings.XyWing()` |
| [XYZ-Wing](http://sudopedia.enjoysudoku.com/XYZ-Wing.html) | `Technique.Wings.XyzWing()` |
| [WXYZ-Wing](http://sudopedia.enjoysudoku.com/WXYZ-Wing.html) | `Technique.Wings.WxyzWing()` |

## Usage

Install [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet) or newer to be able to build the projects.

``` sh
cd src/SudokuConsole
dotnet build
```

To solve a Sudoku completely, just provide the path to a Sudoku input file.

``` sh
./bin/Debug/net5.0/SudokuConsole.exe -i ../../inputs/sudoku-basic1.txt
```

To solve only the next step of an in-progress sudoku, you can use the `--nextStep` flag.

``` sh
./bin/Debug/net5.0/SudokuConsole.exe -i ../../inputs/withCandidates/sudoku-evil1.txt --nextStep
```
