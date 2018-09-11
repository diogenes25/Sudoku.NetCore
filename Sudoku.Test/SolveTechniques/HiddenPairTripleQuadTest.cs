using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class HiddenPairTripleQuadTest
    {
        private static ASolveTechnique[] solveTechniques;
        private IBoard _board;

        #region Additional test attributes

        [TestInitialize]
        public void Initialize()
        {
            this._board = new Board(solveTechniques);
        }
        

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            solveTechniques = GetSolveTechniques();
        }

        #endregion Additional test attributes

        private static ASolveTechnique[] GetSolveTechniques()
        {
            return new ASolveTechnique[] {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad()
            };
        }

        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_and_HiddenPairTripleQuad_Test()
        {
            SudokuLog log = this._board.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(this._board.IsComplete);
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual((i + 1), this._board[i].Digit);
            }
        }
    }
}