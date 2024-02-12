using System;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test.Extensions
{
    [TestClass]
    public class ExtenstionsTests
    {
        [TestMethod]
        public void MatrixTest()
        {
            var board = Board.PureBoard();

            // Set 2 Digit with cellID
            board.SetDigit(cellID: 1, digitToSet: 1);

            // Read cell (with id 2) and set digit (2) of the cell.
            board[2].SetDigit(digit: 2);

            // Board-Extension "Matrix" to create a ASCII-Matrix of the board.
            var outp = board.Matrix();
            Assert.IsTrue(outp.Length > 100);
            Assert.IsNotNull(outp);

            // Show Board in console.
            Console.WriteLine(outp);

            outp = board.MatrixWithCandidates();
            Console.WriteLine(outp);
            Assert.IsTrue(outp.Length > 100);
            Assert.IsNotNull(outp);
        }

        [TestMethod]
        public void MatrixWithCandidatesTest()
        {
            var board = Board.PureBoard();

            // Set 2 Digit with cellID
            board.SetDigit(cellID: 1, digitToSet: 1);

            // Read cell (with id 2) and set digit (2) of the cell.
            board[2].SetDigit(digit: 2);

            // Board-Extension "Matrix" to create a ASCII-Matrix of the board.
            var outp = board.MatrixWithCandidates();

            Console.WriteLine(outp);
            Assert.IsTrue(outp.Length > 100);
            Assert.IsNotNull(outp);
        }

        [TestMethod]
        public void SetCellsFromStringTest()
        {
            var board = Board.PureBoard();
            var boardAsStr = "123000000456000000700000000000000000000000000000000000000000000000000000000000000";
            board.SetCellsFromString(boardAsStr);
            Assert.AreEqual(1, board[0].Digit);
        }

        [TestMethod]
        public void SetDigitTest()
        {
            var board = Board.PureBoard();
            board.SetDigit(row: 0, col: 0, digit: 1);
            Assert.AreEqual(1, board[0].Digit);
            board.SetDigit(row: 'A', col: 1, digit: 2);
            Assert.AreEqual(2, board[1].Digit);
            board.SetDigit('a', 2, 3);
            Assert.AreEqual(3, board[2].Digit);
        }
    }
}
