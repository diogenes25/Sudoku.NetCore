using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
    [TestClass]
    public class BoardEqualsTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;
        private IBoard<Cell> _board;

        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) => _solveTechniques = new ASolveTechnique<Cell>[]
            {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>(),
            };

        [TestMethod]
        public void Check_Equals_If_Cellvalue_Changes_Test()
        {
            IBoard<Cell> otherBoard = new Board();
            Assert.AreEqual(_board, otherBoard);
            Assert.AreEqual(_board.GetHashCode(), otherBoard.GetHashCode());
            var log = otherBoard[0].SetDigit(1);
            Assert.IsTrue(log.Successful);
            Assert.AreNotEqual(_board, otherBoard);
            Assert.AreNotEqual(_board.GetHashCode(), otherBoard.GetHashCode());
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod]
        public void Clone_Digit_and_Candidates_are_equal_to_clone_Test()
        {
            var expected = new Board();
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                _board.SetDigit(i, i, i + 1);
                expected.SetDigit(i, i, i + 1);
            }
            object actual;
            actual = ((Board)_board).Clone();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Clone_Test()
        {
            var recreatedBoard = (Board)((Board)_board).Clone();
            Assert.AreEqual(_board, recreatedBoard);
            Assert.AreEqual(3, recreatedBoard.SolveTechniques.Count);
            Assert.AreEqual(3, ((Board)_board).SolveTechniques.Count);
            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                Assert.AreEqual(i, recreatedBoard[i].ID);
                Assert.AreEqual(_board[i].CandidateValue, recreatedBoard[i].CandidateValue);
                Assert.AreEqual(_board[i].Digit, recreatedBoard[i].Digit);
                Assert.AreEqual(_board[i].ID, recreatedBoard[i].ID);
            }

            _board.SetDigit(0, 1);
            for (var ccc = 0; ccc < Consts.DIMENSIONSQUARE - 1; ccc++)
            {
                Assert.AreEqual(_board[1].Candidates[ccc], ccc + 2);
            }
        }

        [TestMethod]
        public void Equals_Test()
        {
            IBoard<Cell> otherBoard = new Board();
            var beq = _board.Equals(otherBoard);
            Assert.IsTrue(beq);
            Assert.AreEqual(_board, otherBoard);
        }

        [TestInitialize]
        public void Initialize() => _board = new Board(_solveTechniques);
    }
}
