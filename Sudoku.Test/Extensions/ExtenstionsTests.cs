using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var board = new Board();

            // Set 2 Digit with cellID
            board.SetDigit(cellID: 1, digitToSet: 1);

            // Read cell (with id 2) and set digit (2) of the cell.
            board[2].SetDigit(digitToSet: 2);

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
            var board = new Board();

            // Set 2 Digit with cellID
            board.SetDigit(cellID: 1, digitToSet: 1);

            // Read cell (with id 2) and set digit (2) of the cell.
            board[2].SetDigit(digitToSet: 2);

            // Board-Extension "Matrix" to create a ASCII-Matrix of the board.
            var outp = board.MatrixWithCandidates();

            Console.WriteLine(outp);
            Assert.IsTrue(outp.Length > 100);
            Assert.IsNotNull(outp);
        }

        [TestMethod]
        public void SetCellsFromStringTest()
        {
            var board = new Board();
            var boardAsStr = "123000000456000000700000000000000000000000000000000000000000000000000000000000000";
            board.SetCellsFromString(boardAsStr);
            Assert.IsTrue(board[0].Digit == 1);
        }

        [TestMethod]
        public void SetDigitTest()
        {
            var board = new Board();
            board.SetDigit(0, 0, 1);
            Assert.IsTrue(board[0].Digit == 1);
            board.SetDigit('A', 1, 2);
            Assert.IsTrue(board[1].Digit == 2);
            board.SetDigit('a', 2, 3);
            Assert.IsTrue(board[2].Digit == 3);
        }
    }
}
