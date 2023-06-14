using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class NakedPairTrippleQuadTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) => _solveTechniques = new ASolveTechnique<Cell>[] {
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>()
            };

        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_and_NakedPairTrippleQuadTest_Test()
        {
            var target = new Board(_solveTechniques);
            var log = target.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(target.IsComplete());
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual((i + 1), target[i].Digit);
            }
        }

        /// <summary>
        /// 8,9  aus Cell[30] bis Cell[35] löschen.
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// 123000000
        /// 456000000
        /// 700000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void NakedPairTrippleQuadTest_in_Box_Test()
        {
            IBoard<Cell> board = new Board(_solveTechniques);
            board.SetCellsFromString("123000000456000000700000000000000000000000000000000000000000000000000000000000000");
            // 8,9 sind in der Row[2] komplett gesetzt, obwohl diese beiden Digit nur in den Cellen 28 und 29 sein können.
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsTrue(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(8));
                Assert.IsTrue(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(9));
            }
            board.StartSolve();
            // 8,9 sind in jetzt aus Cell[30] bis Cell[35].
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsFalse(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(8));
                Assert.IsFalse(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(9));
            }
        }

        /// <summary>
        /// 8,9  aus Cell[30] bis Cell[35] löschen.
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// 123000000
        /// 056000000
        /// 089000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void NakedPairTrippleQuadTest_in_Col_Test()
        {
            IBoard<Cell> board = new Board(_solveTechniques);
            board.SetCellsFromString("123000000056000000089000000000000000000000000000000000000000000000000000000000000");
            // 4,7 sind in der Col[1] komplett gesetzt, obwohl diese beiden Digit nur in den Cellen 28 und 29 sein können.
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsTrue(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(4));
                Assert.IsTrue(board.GetHouse(HouseType.Row, 2)[i].Candidates.Contains(7));
            }
            board.StartSolve();
            // 8,9 sind in jetzt aus Cell[30] bis Cell[35].
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsFalse(board.GetHouse(HouseType.Col, 0)[i].Candidates.Contains(4));
                Assert.IsFalse(board.GetHouse(HouseType.Col, 0)[i].Candidates.Contains(7));
            }
        }

        /// <summary>
        /// 8,9  aus Cell[30] bis Cell[35] löschen.
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// 123456700
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void NakedPairTrippleQuadTest_in_Row_Test()
        {
            IBoard<Cell> board = new Board(_solveTechniques);
            board.SetCellsFromString("123456700000000000000000000000000000000000000000000000000000000000000000000000000");
            var block1r2Value = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 7) | (1 << 8);
            // 8,9 sind in der Box[2] komplett gesetzt, obwohl diese beiden Digit nur in den Cellen 7 und 8 sein können.
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsTrue(board.GetHouse(HouseType.Box, 2)[i].Candidates.Contains(8));
                Assert.IsTrue(board.GetHouse(HouseType.Box, 2)[i].Candidates.Contains(9));
                Assert.AreEqual(block1r2Value, board.GetHouse(HouseType.Box, 2)[i].CandidateValue);
            }
            board.StartSolve();
            // 8,9 sind in jetzt aus Cell[30] bis Cell[35].

            block1r2Value = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);
            for (var i = 3; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.IsFalse(board.GetHouse(HouseType.Box, 2)[i].Candidates.Contains(8));
                Assert.IsFalse(board.GetHouse(HouseType.Box, 2)[i].Candidates.Contains(9));
                Assert.AreEqual(block1r2Value, board.GetHouse(HouseType.Box, 2)[i].CandidateValue);
            }
        }
    }
}
