﻿# NakedPair

## Function

**Example:**
NakedPair in row0.
The digit 8 and 9 can only be in Cell7 and  Cell8.
So the candidates 8 and 8 must be removed from every other cell in Box2.

```csharp
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.SolveTechniques;

var board = new Board();

board.SetCellsFromString("123456700000000000000000000000000000000000000000000000000000000000000000000000000");
// Show init Board
System.Console.WriteLine("Given Board with candidates:");
System.Console.WriteLine(board.MatrixWithCandidates());

// Create 
var nakedPair = new NakedPairTrippleQuad<Cell>();

// Start Solve with first row.
nakedPair.SolveHouse(board, board.GetHouse(EHouseType.Box, 2), new SudokuLog());

// Board after solving
System.Console.WriteLine("Board with removed candidates 8 and 9 in Box2");
System.Console.WriteLine(board.MatrixWithCandidates());
```

```console
 Given Board with candidates:
   1   2   3    4   5   6    7   8   9
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │   │   │   ││   │   │   ││   │   │   │
A│ 1 │ 2 │ 3 ││ 4 │ 5 │ 6 ││ 7 │   │   │
 │   │   │   ││   │   │   ││   │ 89│ 89│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │   │   │   ││123│123│123││123│123│123│
B│456│456│456││   │   │   ││456│456│456│
 │789│789│789││789│789│789││ 89│ 89│ 89│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │   │   │   ││123│123│123││123│123│123│
C│456│456│456││   │   │   ││456│456│456│
 │789│789│789││789│789│789││ 89│ 89│ 89│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │ 23│1 3│12 ││123│123│123││123│123│123│
D│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
E│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
F│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │ 23│1 3│12 ││123│123│123││123│123│123│
G│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
H│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
I│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘

Complete: 22,085048010973935 %

Board with removed candidates in Box2
   1   2   3    4   5   6    7   8   9
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │   │   │   ││   │   │   ││   │   │   │
A│ 1 │ 2 │ 3 ││ 4 │ 5 │ 6 ││ 7 │   │   │
 │   │   │   ││   │   │   ││   │ 89│ 89│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │   │   │   ││123│123│123││123│123│123│
B│456│456│456││   │   │   ││456│456│456│
 │789│789│789││789│789│789││   │   │   │
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │   │   │   ││123│123│123││123│123│123│
C│456│456│456││   │   │   ││456│456│456│
 │789│789│789││789│789│789││   │   │   │
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │ 23│1 3│12 ││123│123│123││123│123│123│
D│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
E│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
F│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │ 23│1 3│12 ││123│123│123││123│123│123│
G│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
H│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │ 23│1 3│12 ││123│123│123││123│123│123│
I│456│456│456││ 56│4 6│45 ││456│456│456│
 │789│789│789││789│789│789││ 89│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘

Complete: 23,731138545953357 %
```