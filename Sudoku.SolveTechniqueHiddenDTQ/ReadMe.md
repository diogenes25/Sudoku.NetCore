﻿# Hidden Pairs/Triples/...

https://sudoku.com/sudoku-rules/hidden-pairs/

https://sudoku.com/sudoku-rules/hidden-triples/

```csharp
var board = new Board();

board.SetCellsFromString("000000000000000078000780000007000000008000000000000000000000000000000000000000000");
// Show init Board
System.Console.WriteLine("Given Board with candidates:");
System.Console.WriteLine(board.MatrixWithCandidates());

// Create solvetechnique
var hidden = new HiddenPairTripleQuad<Cell>();

// Start Solve with first row.
hidden.SolveHouse(board, board.GetHouse(EHouseType.Box, 0), new SudokuLog());

// Init Board
System.Console.WriteLine("Board with candidates 7 and 8 left in Cell[A|1] and Cell[A|2]");
System.Console.WriteLine(board.MatrixWithCandidates());
```

Output:

```console
 Given Board with candidates:
   1   2   3    4   5   6    7   8   9
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│123│123││123│123│123││123│123│123│
A│456│456│456││456│456│456││456│456│456│
 │789│789│  9││  9│  9│  9││  9│  9│  9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│   │   │
B│456│456│456││456│456│456││456│ 7 │ 8 │
 │  9│  9│  9││  9│  9│  9││  9│   │   │
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││   │   │123││123│123│123│
C│456│456│456││ 7 │ 8 │456││456│456│456│
 │  9│  9│  9││   │   │  9││  9│  9│  9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│123│   ││123│123│123││123│123│123│
D│456│456│ 7 ││456│456│456││456│456│456│
 │  9│  9│   ││ 89│  9│ 89││ 89│ 89│  9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│   ││123│123│123││123│123│123│
E│456│456│ 8 ││456│456│456││456│456│456│
 │  9│  9│   ││  9│7 9│7 9││7 9│  9│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
F│456│456│456││456│456│456││456│456│456│
 │  9│  9│  9││ 89│7 9│789││789│ 89│7 9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│123│123││123│123│123││123│123│123│
G│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
H│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
I│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘

Complete: 20,301783264746234 %

Board with candidates 7 and 8 left in Cell[A|1] and Cell[A|2]
   1   2   3    4   5   6    7   8   9
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │   │   │123││123│123│123││123│123│123│
A│   │   │456││456│456│456││456│456│456│
 │78 │78 │  9││  9│  9│  9││  9│  9│  9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│   │   │
B│456│456│456││456│456│456││456│ 7 │ 8 │
 │  9│  9│  9││  9│  9│  9││  9│   │   │
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││   │   │123││123│123│123│
C│456│456│456││ 7 │ 8 │456││456│456│456│
 │  9│  9│  9││   │   │  9││  9│  9│  9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│123│   ││123│123│123││123│123│123│
D│456│456│ 7 ││456│456│456││456│456│456│
 │  9│  9│   ││ 89│  9│ 89││ 89│ 89│  9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│   ││123│123│123││123│123│123│
E│456│456│ 8 ││456│456│456││456│456│456│
 │  9│  9│   ││  9│7 9│7 9││7 9│  9│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
F│456│456│456││456│456│456││456│456│456│
 │  9│  9│  9││ 89│7 9│789││789│ 89│7 9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│123│123││123│123│123││123│123│123│
G│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
H│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│123│123││123│123│123││123│123│123│
I│456│456│456││456│456│456││456│456│456│
 │789│789│  9││ 89│7 9│789││789│ 89│7 9│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘

Complete: 22,222222222222214 %
```