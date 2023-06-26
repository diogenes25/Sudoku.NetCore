using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku.NetCore;

namespace Sudoku.Test.SolveTechniques
{
    [TestClass]
    public class LastCandidateInHouseTechiquesTests
    {
        [TestMethod]
        public void Check_Last_Digit_Box_Simple_Test()
        {
            var board = new Board();
            board.SetCellsFromString("000056789004000000000000000000000000000000000000000000000000000000000000000000000");
            Assert.AreEqual(0, board[0].Digit);
            Assert.AreEqual(0, board[3].Digit);
            var firRow = board.GetHouse(EHouseType.Row, 0);
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            Console.WriteLine(board.MatrixWithCandidates());
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            Assert.AreEqual(4, board[3].Digit);
        }
        [TestMethod]
        public void Check_Last_Digit_Test()
        {
            var board = new Board();
            board.SetCellsFromString("000056789004000000000000000000000000000000000000000000000000000000000000000000000");
            Assert.AreEqual(0, board[0].Digit);
            Assert.AreEqual(0, board[3].Digit);
            var firRow = board.GetHouse(EHouseType.Row, 0);
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            Console.WriteLine(board.MatrixWithCandidates());
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            Assert.AreEqual(4, board[3].Digit);
        }

        [TestMethod]
        public void CheckLastDigit_Row_Double_Test()
        {
            var board = new Board();
            board.SetCellsFromString(
"000056089" +
"000000000" +
"074000000" +
"000000000" +
"000000000" +
"000000000" +
"000700400" +
"000000000" +
"000000000");
            Assert.IsTrue(board[3].Candidates.Contains(4));
            Assert.IsTrue(board[6].Candidates.Contains(7));
            var firRow = board.GetHouse(EHouseType.Row, 0);
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            Console.WriteLine(board.MatrixWithCandidates());
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            Assert.AreEqual(4, board[3].Digit);
            Assert.AreEqual(7, board[6].Digit);
        }

        [TestMethod]
        public void CheckLastDigit_Row_Test()
        {
            var board = new Board();
            board.SetCellsFromString("030050040008010500460000012070502080000603000040109030250000098001020600080060020");
            Assert.IsTrue(board[0].Candidates.Contains(1));
            var firstBox = board.GetHouse(EHouseType.Row, 0);
            Console.WriteLine(board.MatrixWithCandidates());
            // Findet zuviele LASTDIGIT!!! Auch 8 und 9
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            lastCandidateInHouse.SolveHouse(board, firstBox, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            //Assert.IsTrue(firstBox.IsComplete());
            Assert.AreEqual(1, board[0].Digit);
        }
    }
}
