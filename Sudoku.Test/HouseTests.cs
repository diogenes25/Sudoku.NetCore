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
            var board = Board.PureBoard();
            board.SetCellsFromString("023456789000000000000000000000000000000000000000000000000000000000000000000000000");
            Assert.AreEqual(1, board[0].Digit);
            var firRow = board.GetHouse(EHouseType.Row, 0);
            var result = ((House)firRow).CheckLastDigit(new SudokuLog());
            Assert.IsTrue(result);
        }
    }
}
