using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
    [TestClass]
    public class HouseTests
    {
        [TestMethod]
        public void Check_Last_Digit_Test()
        {
            var board = new Board();
            board.SetCellsFromString("000056789004000000000000000000000000000000000000000000000000000000000000000000000");
            Assert.AreEqual(0, board[0].Digit);
            Assert.AreEqual(0, board[3].Digit);
            var firRow = board.GetHouse(EHouseType.Row, 0);
            ((House)firRow).CheckLastDigit(new SudokuLog());
            //Assert.AreEqual(4, board[3].Digit);
        }
    }
}
