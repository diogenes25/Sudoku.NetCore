<img src="Sudoku_Net-CoreV1_320x160.png" width="320">

# Sudoku.NetCore

[![Build status](https://ci.appveyor.com/api/projects/status/p3ke0nr0fhpk2o7f?svg=true)](https://ci.appveyor.com/project/diogenes25/sudoku-netcore-jlyw2)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/82eefb090e91426c9c293856039efb15)](https://app.codacy.com/gh/diogenes25/Sudoku.NetCore/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![CodeQL](https://github.com/diogenes25/Sudoku.NetCore/actions/workflows/codeql.yml/badge.svg)](https://github.com/diogenes25/Sudoku.NetCore/actions/workflows/codeql.yml)

## Dot.Net Core implementation of a Sudoku-Solver

Solves Sudoku.

# Rules

https://en.wikipedia.org/wiki/Sudoku

# Definition

**Board** = contains 81 cells organized in 9 rows, 9 columns and 9 boxes.

**Cell** = Has one digit or 1 to 9 candidates

**Box** = square of 3*3 cells

**Row** = 9 cells in a row

**Column** = 9 cells in a column

**Digit** = One given Number (1 to 9) in a Cell

**Candidates** = Possible Digits (up to 9)


<img src="SudokuCRC.png">

# Background

A Sudoku has 81 cells, where each cell is part of a box, a row and a column.
At the beginning, each cell can receive a number between 1 and 9. 
After a cell has received a digit, this possible value is reduced for each cell that is also in the same box, column and row as the corresponding cell.

If the reduction of possibilities leaves only one possible number in a cell, column or row, this remaining number is set and thus starts a further reduction of possibilities for the affected cells.

Besides the simple reduction of the candidates, there are several other strategies that reduce the possibilities even further.


