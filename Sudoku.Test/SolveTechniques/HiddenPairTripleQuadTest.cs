using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class HiddenPairTripleQuadTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;
        private IBoard<Cell> _board;

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) => _solveTechniques = GetSolveTechniques();

        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_and_HiddenPairTripleQuad_Test()
        {
            var log = _board.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(_board.IsComplete());
            for (var i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual((i + 1), _board[i].Digit);
            }
        }

        [TestInitialize]
        public void Initialize() => _board = new Board(_solveTechniques);

        private static ASolveTechnique<Cell>[] GetSolveTechniques() => new ASolveTechnique<Cell>[] {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>()
            };
    }
}
