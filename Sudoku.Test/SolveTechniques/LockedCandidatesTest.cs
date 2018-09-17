using DE.Onnen.Sudoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class LockedCandidatesTest
    {
        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_and_LockedCandidates_Test()
        {
            Board target = new Board(new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>());
            SudokuLog log = target.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(target.IsComplete());
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual((i + 1), target[i].Digit);
            }
        }
    }
}