using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
    [TestClass]
    public class BoardExtensionsTests
    {
        [TestMethod]
        public void ExtractCellsToString_Init_Test()
        {
            var board = Board.PureBoard();

            var resultStr = board.ExtractCellsToString();
            Assert.AreEqual(resultStr.Length, board.Count);
            Assert.AreEqual(resultStr, "000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void ExtractCellsToString_Test()
        {
            var board = Board.PureBoard();
            board.SetDigit(0, 0, 1);
            var resultStr = board.ExtractCellsToString();

            Assert.AreEqual(resultStr.Length, board.Count);
            Assert.AreEqual(resultStr, "100000000000000000000000000000000000000000000000000000000000000000000000000000000");

            board.SetDigit(1, 2);
            resultStr = board.ExtractCellsToString();

            Assert.AreEqual(resultStr.Length, board.Count);
            Assert.AreEqual(resultStr, "120000000000000000000000000000000000000000000000000000000000000000000000000000000");
        }
    }
}
