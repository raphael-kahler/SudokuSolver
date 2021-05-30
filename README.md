# Sudoku Solver

A solver for Sudoku puzzles that uses common [solving techniques](http://sudopedia.enjoysudoku.com/Solving_Technique.html).
The solver shows for every step which technique was used, so you can follow along how the puzzle was solved.
You can use this for learning techniques or to get a hint for the sudoku you are stuck on.

![Console Output Example](https://i.imgur.com/US62oen.png)

## Techniques

The following techniques are implemented so far. Each technique can individually be added to the solver.

| Technique |  Usage |
| --------- |  ----- |
| **Basic Techniques** | |
| Each value can only appear once per row/column/box. | `EliminationByValue.AllDirections()` |
| [Naked Single](http://sudopedia.enjoysudoku.com/Naked_Single.html) | `NakedSubset.NakedSingle()` |
| [Naked Pair](http://sudopedia.enjoysudoku.com/Naked_Pair.html) | `NakedSubset.NakedPairs()` |
| [Naked Triple](http://sudopedia.enjoysudoku.com/Naked_Triple.html) | `NakedSubset.NakedTriples()` |
| [Naked Quad](http://sudopedia.enjoysudoku.com/Naked_Quad.html) | `NakedSubset.NakedQuads()` |
| [Hidden Single](http://sudopedia.enjoysudoku.com/Hidden_Single.html) | `HiddenSubset.HiddenSingles()` |
| [Hidden Pair](http://sudopedia.enjoysudoku.com/Hidden_Pair.html) | `HiddenSubset.HiddenPairs()` |
| [Hidden Triple](http://sudopedia.enjoysudoku.com/Hidden_Triple.html) | `HiddenSubset.HiddenTriples()` |
| [Hidden Quad](http://sudopedia.enjoysudoku.com/Hidden_Quad.html) | `HiddenSubset.HiddenQuads()` |
| **Intersection Techniques** | |
| [Locked Candidates Pointing](http://sudopedia.enjoysudoku.com/Locked_Candidates.html) | `LockedCandidatesPointing.AllDirections()` |
| [Locked Candidates Claiming](http://sudopedia.enjoysudoku.com/Locked_Candidates.html) | `LockedCandidateClaiming.AllDirections()` |
| **Fish Techniques** | |
| 2-Fish: [X-Wing](http://sudopedia.enjoysudoku.com/X-Wing.html), and finned variants like [Sashimi X-Wing](http://sudopedia.enjoysudoku.com/Finned_X-Wing.html). | `FishTechnique.XWing()` |
| 3-Fish: [SwordFish](http://sudopedia.enjoysudoku.com/Swordfish.html) and finned variants. |  `FishTechnique.Swordfish()` |
| 4-Fish: [Jellyfish](http://sudopedia.enjoysudoku.com/Jellyfish.html) and finned variants. |  `FishTechnique.Jellyfish()` |
| **Wing Techniques** | |
| [XY-Wing](http://sudopedia.enjoysudoku.com/XY-Wing.html) | `WingTechnique.XyWing()` |
| [XYZ-Wing](http://sudopedia.enjoysudoku.com/XYZ-Wing.html) | `WingTechnique.XyzWing()` |
| [WXYZ-Wing](http://sudopedia.enjoysudoku.com/WXYZ-Wing.html) | `WingTechnique.WxyzWing()` |

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
