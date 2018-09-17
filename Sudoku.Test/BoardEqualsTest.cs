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

        [TestInitialize]
        public void Initialize()
        {
            this._board = new Board(_solveTechniques);
        }

        [TestMethod]
        public void Equals_Test()
        {
            IBoard<Cell> otherBoard = new Board();
            bool beq = this._board.Equals(otherBoard);
            Assert.IsTrue(beq);
            Assert.AreEqual(this._board, otherBoard);
        }

        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _solveTechniques = new ASolveTechnique<Cell>[]
            {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>(),
            };
        }

        [TestMethod]
        public void Check_Equals_If_Cellvalue_Changes_Test()
        {
            IBoard<Cell> otherBoard = new Board();
            Assert.AreEqual(this._board, otherBoard);
            Assert.AreEqual(this._board.GetHashCode(), otherBoard.GetHashCode());
            SudokuLog log = otherBoard[0].SetDigit(1);
            Assert.IsTrue(log.Successful);
            Assert.AreNotEqual(this._board, otherBoard);
            Assert.AreNotEqual(this._board.GetHashCode(), otherBoard.GetHashCode());
        }

        [TestMethod]
        public void Clone_Test()
        {
            Board recreatedBoard = (Board)((Board)this._board).Clone();
            Assert.AreEqual(this._board, recreatedBoard);
            Assert.AreEqual(3, recreatedBoard.SolveTechniques.Count);
            Assert.AreEqual(3, ((Board)this._board).SolveTechniques.Count);
            for (int i = 0; i < Consts.CountCell; i++)
            {
                Assert.AreEqual(i, recreatedBoard[i].ID);
                Assert.AreEqual(this._board[i].CandidateValue, recreatedBoard[i].CandidateValue);
                Assert.AreEqual(this._board[i].Digit, recreatedBoard[i].Digit);
                Assert.AreEqual(this._board[i].ID, recreatedBoard[i].ID);
            }

            this._board.SetDigit(0, 1);
            for (int ccc = 0; ccc < Consts.DimensionSquare - 1; ccc++)
            {
                Assert.AreEqual(this._board[1].Candidates[ccc], ccc + 2);
            }
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod]
        public void Clone_Digit_and_Candidates_are_equal_to_clone_Test()
        {
            Board expected = new Board();
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                this._board.SetDigit(i, i, i + 1);
                expected.SetDigit(i, i, i + 1);
            }
            object actual;
            actual = ((Board)this._board).Clone();
            Assert.AreEqual(expected, actual);
        }
    }
}