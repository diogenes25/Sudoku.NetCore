# Sudoku Extensions

Extension for the [Sudoku-Board](/)


## Matrix
Create a ASCII-Matrix of the board.

Example:
```csharp
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;

// Create Sudoku-Board
var board = new Board();

// Set 2 Digit
board.SetDigit(1, 1);
board.SetDigit(2, 2);

// Board-Extension "Matrix" to create a ASCII-Matrix of the board.
var outp = board.Matrix();

// Show Board in console.
Console.WriteLine(outp);
```

Output:
```ascii
  123 456 789
 ┌───┬───┬───┐
A│ 12│   │   │
B│   │   │   │
C│   │   │   │
 ├───┼───┼───┤
D│   │   │   │
E│   │   │   │
F│   │   │   │
 ├───┼───┼───┤
G│   │   │   │
H│   │   │   │
I│   │   │   │
 └───┴───┴───┘
Complete: 7,681755829903977 %
```

## MatrixWithCandidates

Create a ASCII-Matrix of the board but shows the candidates as well.

Example:
```csharp
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;

// Create Sudoku-Board
var board = new Board();

// Set 2 Digit
board.SetDigit(1, 1);
board.SetDigit(2, 2);

// Board-Extension "MatrixWithCandidates" to create a ASCII-Matrix of the board with candidates.
var outp = board.MatrixWithCandidates();

// Show Board in console.
Console.WriteLine(outp);
```

Output:
```ascii
   1   2   3    4   5   6    7   8   9
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │  3│   │   ││  3│  3│  3││  3│  3│  3│
A│456│ 1 │ 2 ││456│456│456││456│456│456│
 │789│   │   ││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │  3│  3│  3││123│123│123││123│123│123│
B│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │  3│  3│  3││123│123│123││123│123│123│
C│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│ 23│1 3││123│123│123││123│123│123│
D│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│ 23│1 3││123│123│123││123│123│123│
E│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│ 23│1 3││123│123│123││123│123│123│
F│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
 ┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐
 │123│ 23│1 3││123│123│123││123│123│123│
G│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│ 23│1 3││123│123│123││123│123│123│
H│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 ├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤
 │123│ 23│1 3││123│123│123││123│123│123│
I│456│456│456││456│456│456││456│456│456│
 │789│789│789││789│789│789││789│789│789│
 └───┴───┴───┘└───┴───┴───┘└───┴───┴───┘
Complete: 7,681755829903977 %
```

---

## Set a whole Board at once

Use a string

```csharp
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;

// Create Sudoku-Board
var board = new Board();

// Load a whole board from string
board.SetCellsFromString("100000000000000000000000000000000000000000000000000000000000000000000000000000009");
Console.WriteLine($"Digit of Cell0 (first Cell): {board.First().Digit}");
Console.WriteLine($"Digit of Cell80 (last Cell): {board.Last().Digit}");
```

**Output**

```console
Digit of Cell0 (first Cell): 1
Digit of Cell80 (last Cell): 9
```