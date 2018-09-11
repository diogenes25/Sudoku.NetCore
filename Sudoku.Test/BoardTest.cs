using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    ///This is a test class for BoardTest and is intended
    ///to contain all BoardTest Unit Tests
    ///</summary>
    [TestClass]
    public class BoardTest
    {
        private TestContext testContextInstance;
        private static ASolveTechnique[] _solveTechniques;
        private IBoard _board;

        [TestInitialize]
        public void Initialize()
        {
            this._board = new Board(_solveTechniques);
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }
            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _solveTechniques = GetSolveTechniques();
        }

        #endregion Additional test attributes

        private static ASolveTechnique[] GetSolveTechniques()
        {
            ASolveTechnique[] st = new ASolveTechnique[]
            {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad()
            };
            return st;
        }

        /// <summary>
        /// A test for SetDigit
        /// </summary>
        [TestMethod]
        public void SetDigit_with_coordinates()
        {
            int row = 8;
            int col = 8;
            int digit = 9;
            Assert.AreEqual(0, this._board[80].Digit);
            ((Board)this._board).SetDigit(row, col, digit);
            Assert.AreEqual(9, this._board[80].Digit);
            Assert.AreEqual(0, this._board[80].CandidateValue);
        }

        [TestMethod]
        public void SetDigit_with_Alpha_Row_Coordinates()
        {
            Board tmpBoard = (Board)this._board;
            ICell firstCell = this._board[1];
            Assert.AreEqual(0, firstCell.Digit, "Just to be sure that this value is 0 at first.");
            SudokuLog result = tmpBoard.SetDigit('a', 1, 5);
            Assert.IsTrue(result.Successful);
            firstCell = this._board[1];
            Assert.AreEqual(5, firstCell.Digit, "Cell A/1  is same as 0/1 is same as id=1 and must be set to 5");
            result = tmpBoard.SetDigit('B', 1, 6);
            Assert.IsTrue(result.Successful);
            firstCell = this._board[10];
            Assert.AreEqual(6, firstCell.Digit, "Cell B/1 must be set to 6");
        }

        /// <summary>
        /// A test for SetDigit
        /// </summary>
        [TestMethod]
        public void SetDigit_set_digit_when_only_one_candidate_left()
        {
            this._board.SetDigit(0, 1);
            this._board.SetDigit(1, 2);
            this._board.SetDigit(2, 3);

            this._board.SetDigit(9, 4);
            this._board.SetDigit(10, 5);
            this._board.SetDigit(11, 6);

            this._board.SetDigit(18, 7);
            Assert.AreEqual(0, this._board[20].Digit);
            Assert.AreEqual(((1 << 7) + (1 << 8)), this._board[20].CandidateValue);

            // Now Last
            this._board.SetDigit(19, 8);

            Assert.AreEqual(9, this._board[20].Digit);
            Assert.AreEqual(0, this._board[20].CandidateValue);
        }

        /// <summary>
        ///A test for SetDigit
        ///</summary>
        [TestMethod]
        public void SetDigitTest3()
        {
            for (int i = 0; i < 8; i++)
            {
                this._board.SetDigit(i, i + 1);
                if (i == 0)
                {
                    continue;
                }

                this._board.SetDigit((i * 9), 9 - i);
            }
            Assert.AreEqual(9, this._board[8].Digit);
            Assert.AreEqual(0, this._board[8].CandidateValue);
            Assert.AreEqual(9, this._board[72].Digit);
            Assert.AreEqual(0, this._board[72].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest4()
        {
            Board tmpBoard = (Board)this._board;
            tmpBoard.SetDigit(0, 0, 1);
            tmpBoard.SetDigit(0, 1, 2);
            tmpBoard.SetDigit(0, 2, 3);
            tmpBoard.SetDigit(1, 0, 4);
            tmpBoard.SetDigit(1, 1, 5);
            tmpBoard.SetDigit(1, 3, 7);
            tmpBoard.SetDigit(1, 4, 8);
            SudokuLog result = tmpBoard.SetDigit(1, 5, 9);
            Assert.AreEqual(6, this._board[11].Digit);
            int block0Value = (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block0Value, this._board[18].CandidateValue);
            Assert.AreEqual(block0Value, this._board[19].CandidateValue);
            Assert.AreEqual(block0Value, this._board[20].CandidateValue);
            int block1r2Value = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);
            Assert.AreEqual(block1r2Value, this._board[21].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[22].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[23].CandidateValue);
            this._board.Solve(result);
            int block1r2ValueSolve = (1 << 0) | (1 << 1) | (1 << 2);
            Assert.AreEqual(block1r2ValueSolve, this._board[21].CandidateValue);
            Assert.AreEqual(block1r2ValueSolve, this._board[22].CandidateValue);
            Assert.AreEqual(block1r2ValueSolve, this._board[23].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest5()
        {
            Board tmpBoard = (Board)this._board;
            tmpBoard.SetDigit(1, 4, 1);
            tmpBoard.SetDigit(0, 6, 2);
            tmpBoard.SetDigit(0, 7, 3);
            SudokuLog result = tmpBoard.SetDigit(0, 8, 4);
            this._board.Solve(result);
            int block1r2Value = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block1r2Value, this._board[18].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[19].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[20].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest6()
        {
            Board tmpBoard = (Board)this._board;
            tmpBoard.SetDigit(4, 1, 1);
            tmpBoard.SetDigit(6, 0, 2);
            tmpBoard.SetDigit(7, 0, 3);
            SudokuLog result = tmpBoard.SetDigit(8, 0, 4);
            this._board.Solve(result);
            int block1r2Value = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block1r2Value, this._board[2].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[11].CandidateValue);
            Assert.AreEqual(block1r2Value, this._board[20].CandidateValue);
        }

        [TestMethod]
        public void Solve_Sudoku_solved_with_3_solvetechniques_and_without_backtracking()
        {
            Board board = new Board(_solveTechniques);
            string simpleSudoku = @"030050040
008010500
460000012
070502080
000603000
040109030
250000098
001020600
080060020";

            string[] lines = simpleSudoku.Split('\n');
            for (int y = 0; y < 9; y++)
            {
                string line = lines[y];

                for (int x = 0; x < 9; x++)
                {
                    char currChar = line[x];
                    if (currChar.Equals('0'))
                    {
                        continue;
                    }

                    SudokuLog result = board.SetDigit(y, x, Convert.ToInt32(currChar) - 48);
                    Assert.IsTrue(result.Successful);
                }
            }
            SudokuLog sudokuResult = new SudokuLog();
            Assert.IsFalse(board.IsComplete);
            Assert.IsTrue(sudokuResult.Successful);
            board.Solve(sudokuResult);
            Assert.IsTrue(board.IsComplete);
            Assert.IsTrue(sudokuResult.Successful);
        }

        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod]
        public void SolveTest()
        {
            Board tmpBoard = (Board)this._board;
            tmpBoard.SetDigit(1, 0, 2);
            tmpBoard.SetDigit(1, 2, 3);
            tmpBoard.SetDigit(1, 3, 6);
            tmpBoard.SetDigit(1, 7, 9);
            tmpBoard.SetDigit(2, 0, 9);
            tmpBoard.SetDigit(2, 1, 5);
            tmpBoard.SetDigit(2, 5, 3);
            tmpBoard.SetDigit(2, 7, 2);
            tmpBoard.SetDigit(3, 3, 5);
            tmpBoard.SetDigit(3, 6, 3);
            tmpBoard.SetDigit(4, 0, 7);
            tmpBoard.SetDigit(4, 3, 3);
            tmpBoard.SetDigit(4, 4, 8);
            tmpBoard.SetDigit(4, 5, 6);
            tmpBoard.SetDigit(4, 8, 1);
            tmpBoard.SetDigit(5, 2, 6);
            tmpBoard.SetDigit(5, 5, 7);
            tmpBoard.SetDigit(6, 1, 7);
            tmpBoard.SetDigit(6, 3, 1);
            tmpBoard.SetDigit(6, 7, 3);
            tmpBoard.SetDigit(6, 8, 4);
            tmpBoard.SetDigit(7, 1, 4);
            tmpBoard.SetDigit(7, 5, 9);
            tmpBoard.SetDigit(7, 6, 5);
            tmpBoard.SetDigit(7, 8, 7);

            SudokuLog result = tmpBoard.SetDigit(0, 0, 6);
            this._board.Solve(result);
            Assert.AreEqual(2, this._board[80].Digit);
        }

        /// <summary>
        ///A test for Board Constructor
        ///</summary>
        [TestMethod]
        public void BoardConstructor_whith_null_techniques_cells_must_be_set()
        {
            ASolveTechnique[] tempSolveTechniques = null;
            IBoard tmpBoard = new Board(tempSolveTechniques);
            CheckBoard(tmpBoard);
        }

        public void BoardConstructor_whith_techniques_cells_must_be_set()
        {
            CheckBoard(this._board);
        }

        private static void CheckBoard(IBoard target)
        {
            Assert.AreEqual(Consts.DimensionSquare * Consts.DimensionSquare, ((Board)target).Count);
            foreach (ICell cell in target)
            {
                Assert.AreEqual(Consts.BaseStart, cell.CandidateValue);
                Assert.AreEqual(0, cell.Digit);
            }
        }

        /// <summary>
        ///A test for Board Constructor
        ///</summary>
        [TestMethod]
        public void Constructor_empty_cells_must_be_set()
        {
            CheckBoard(this._board);
        }

        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit()
        {
            SudokuLog log = this._board.Backtracking();
            Assert.IsTrue(this._board.IsComplete);
            Assert.IsTrue(log.Successful);
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual((i + 1), this._board[i].Digit);
            }
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod]
        public void Clear_not_digit_is_set()
        {
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                ((Board)this._board).SetDigit(i, i, i + 1);
                Assert.AreEqual(i + 1, this._board[i + (i * Consts.DimensionSquare)].Digit);
            }
            this._board.Clear();
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual(0, this._board[i].Digit);
                Assert.AreEqual(0, this._board[i + (i * Consts.DimensionSquare)].Digit);
                Assert.AreEqual(0, this._board[Consts.DimensionSquare - (i + (Consts.DimensionSquare - (i * Consts.DimensionSquare)))].Digit);
            }
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod]
        public void Clone_Digit_and_Candidates_are_equal_to_clone()
        {
            Board expected = new Board();
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                ((Board)this._board).SetDigit(i, i, i + 1);
                expected.SetDigit(i, i, i + 1);
            }
            object actual;
            actual = ((Board)this._board).Clone();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateSimpleBoard
        ///</summary>
        [TestMethod]
        public void CreateSimpleBoard_creates_int_with()
        {
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                this._board.SetDigit(i, i + 1);
            }
            int[] expected = { -1, -2, -3, -4, -5, -6, -7, -8, -9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] actual;
            actual = Board.CreateSimpleBoard(this._board);
            Assert.AreEqual(expected.Length, actual.Length);
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            IEnumerator<ICell> actual;
            actual = this._board.GetEnumerator();
            int i = 0;
            while (actual.MoveNext())
            {
                ICell c = actual.Current;
                Assert.AreEqual(c, this._board[i++]);
            }
        }

        /// <summary>
        ///A test for GetHouse
        ///</summary>
        [TestMethod]
        public void GetHouse_House_Row()
        {
            HouseType houseType = HouseType.Row;
            IHouse actual;
            for (int idx = 0; idx < Consts.DimensionSquare; idx++)
            {
                actual = this._board.GetHouse(houseType, idx);
                for (int r = 0; r < Consts.DimensionSquare; r++)
                {
                    ICell expected = actual[r];
                    ICell cellRow = this._board[r + (idx * Consts.DimensionSquare)];
                    Assert.AreEqual(cellRow, expected);
                }
            }
        }

        /// <summary>
        ///A test for GetHouse
        ///</summary>
        [TestMethod]
        public void GetHouse_House_Col()
        {
            HouseType houseType = HouseType.Col;
            IHouse actual;
            for (int idx = 0; idx < Consts.DimensionSquare; idx++)
            {
                actual = this._board.GetHouse(houseType, idx);
                for (int r = 0; r < Consts.DimensionSquare; r++)
                {
                    ICell expected = actual[r];
                    ICell cellRow = this._board[idx + (r * Consts.DimensionSquare)];
                    Assert.AreEqual(cellRow, expected);
                }
            }
        }

        /// <summary>
        ///A test for SetBoard
        ///</summary>
        [TestMethod]
        public void SetBoard_set_Digit_at_cell_0()
        {
            IBoard otherBoard = new Board();
            otherBoard.SetDigit(0, 1);
            Assert.AreEqual(0, this._board[0].Digit);
            ((Board)this._board).SetBoard(otherBoard);
            Assert.AreEqual(1, this._board[0].Digit);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToString_is_string_with_every_digit()
        {
            string expected = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            string actual;
            actual = this._board.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod]
        public void Count_is_length_of_cells()
        {
            int actual;
            actual = ((Board)this._board).Count;
            Assert.AreEqual(Consts.DimensionSquare * Consts.DimensionSquare, actual);
        }

        /// <summary>
        ///A test for Givens
        ///</summary>
        [TestMethod]
        public void GivensTest()
        {
            ReadOnlyCollection<ICell> actual;
            actual = this._board.Givens;
            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        ///A test for IsComplete
        ///</summary>
        [TestMethod]
        public void IsComplete_board_is_false_at_first()
        {
            bool actual;
            actual = this._board.IsComplete;
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod]
        public void ItemTest()
        {
            int index = 0; 
            ICell actual;
            actual = this._board[index];
            Assert.AreEqual(0, actual.ID);
        }

        /// <summary>
        ///A test for SolvePercent
        ///</summary>
        [TestMethod]
        public void SolvePercent_is_0_at_first()
        {
            double actual;
            actual = this._board.SolvePercent;
            Assert.AreEqual(0.0, actual);
        }

        [TestMethod]
        public void SolvePercent_is_greater_than_0()
        {
            double actual;
            this._board.SetDigit(0, 1);
            actual = this._board.SolvePercent;
            Assert.IsTrue(actual > 0.0);
        }

        /// <summary>
        /// Produce an error.
        /// </summary>
        /// <remarks>
        /// 003X0000
        /// 456000000
        /// 789000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void SetDigit_LastDigit_produce_an_error()
        {
            this._board.SetCellsFromString("003000000456000000789000000000000000000000000000000000000000000000000000000000000");
            // Es ist möglich die 1 oder 2 in Zelle 3 zu setzen. Dies führt aber zu einem Fehler.
            SudokuLog log = this._board.SetDigit(3, 1);
            Assert.IsFalse(log.Successful);
        }
    }
}