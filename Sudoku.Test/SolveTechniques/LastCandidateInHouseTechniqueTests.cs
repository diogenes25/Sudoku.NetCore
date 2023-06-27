using System;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku.NetCore;

namespace Sudoku.Test.SolveTechniques
{
    [TestClass]
    public class LastCandidateInHouseTechniqueTests
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
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
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
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
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
            lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
            Assert.AreEqual(4, board[3].Digit);
            Assert.AreEqual(7, board[6].Digit);
        }

        [TestMethod]
        public void Board_HighScholl_Level_Test()
        {
            var board = new Board();
            board.AddSolveTechnique(new LastCandidateInHouseTechiques());
            board.SetCellsFromString(
"050720300" +
"320000000" +
"100004000" +
"200500100" +
"800900006" +
"076000000" +
"009000000" +
"000001007" +
"005300009");
            var log = new SudokuLog();
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();

            for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
            {
                for (var containerType = 0; containerType < 3; containerType++)
                {
                    lastCandidateInHouse.SolveHouse(board, board.GetHouse((EHouseType)containerType, containerIdx), log);
                    Assert.IsTrue(log.Successful, $"Error: {containerType} {containerIdx} = {log.ToString()}");
                }
            }
            Console.WriteLine(board.MatrixWithCandidates());

            log = new SudokuLog();
            var thirdCol = board.GetHouse(EHouseType.Row, 4);
            lastCandidateInHouse.SolveHouse(board, thirdCol, log);
            Assert.IsTrue(log.Successful);
        }

        [TestMethod]
        public void Simple_Sudoku_WithoutSolveTechnique_Test()
        {
            var board = new Board();
            board.SetCellsFromString(
"000480200" +
"050010907" +
"106000030" +
"002600000" +
"090100850" +
"300700040" +
"000502006" +
"805090700" +
"004067001");
            //Assert.IsTrue(board[3].Candidates.Contains(4));
            //Assert.IsTrue(board[6].Candidates.Contains(7));
            //var firRow = board.GetHouse(EHouseType.Row, 0);
            //var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            //Console.WriteLine(board.MatrixWithCandidates());
            //lastCandidateInHouse.SolveHouse(board, firRow, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            //Assert.AreEqual(4, board[3].Digit);
            Assert.IsTrue(board.IsComplete());
        }

        [TestMethod]
        public void CheckLastDigit_Row_Test()
        {
            var board = new Board();
            board.SetCellsFromString("030050040008010500460000012070502080000603000040109030250000098001020600080060020");
            Assert.IsTrue(board[0].Candidates.Contains(1));
            var firstBox = board.GetHouse(EHouseType.Row, 0);
            Console.WriteLine(board.MatrixWithCandidates());
            var lastCandidateInHouse = new LastCandidateInHouseTechiques();
            lastCandidateInHouse.SolveHouse(board, firstBox, new SudokuLog());
            Console.WriteLine(board.MatrixWithCandidates());
            Assert.AreEqual(1, board[0].Digit);
        }
    }
}
