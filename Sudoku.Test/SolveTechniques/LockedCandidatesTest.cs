using DE.Onnen.Sudoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class LockedCandidatesTest
    {
        /// <summary>
        /// A test for Backtracking
        /// </summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_and_LockedCandidates_Test()
        {
            var target = new Board().AddSolveTechnique(new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>());
            var log = target.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(target.IsComplete());
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual((i + 1), target[i].Digit);
            }
        }
    }
}
