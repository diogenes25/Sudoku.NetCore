namespace DE.Onnen.Sudoku
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DE.Onnen.Sudoku.Extensions;
    using DE.Onnen.Sudoku.SolveTechniques;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for BoardTest and is intended
    /// to contain all BoardTest Unit Tests
    ///</summary>
    [TestClass]
    public class BoardTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;
        private IBoard<Cell> _board;

        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void BoardTestInitialize(TestContext testContext) => _solveTechniques = GetSolveTechniques();

        /// <summary>
        ///A test for Backtracking
        ///</summary>
        [TestMethod]
        public void Backtracking_solve_without_any_digit_Test()
        {
            Assert.IsFalse(_board.IsComplete());
            Assert.IsTrue(_board.Backtracking().Successful);
            Assert.IsTrue(_board.IsComplete());
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual((i + 1), _board[i].Digit);
            }
        }

        /// <summary>
        ///A test for Board Constructor
        ///</summary>
        [TestMethod]
        public void BoardConstructor_whith_null_techniques_cells_must_be_set_Test()
        {
            ASolveTechnique<Cell>[] tempSolveTechniques = null;
            IBoard<Cell> tmpBoard = new Board(tempSolveTechniques);
            CheckBoard(tmpBoard);
        }

        public void BoardConstructor_whith_techniques_cells_must_be_set() => CheckBoard(_board);

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod]
        public void Clear_not_digit_is_set_Test()
        {
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                _board.SetDigit(i, i, i + 1);
                Assert.AreEqual(i + 1, _board[i + (i * Consts.DIMENSIONSQUARE)].Digit);
            }
            _board.Clear();
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual(0, _board[i].Digit);
                Assert.AreEqual(0, _board[i + (i * Consts.DIMENSIONSQUARE)].Digit);
                Assert.AreEqual(0, _board[Consts.DIMENSIONSQUARE - (i + (Consts.DIMENSIONSQUARE - (i * Consts.DIMENSIONSQUARE)))].Digit);
            }
        }

        /// <summary>
        ///A test for Board Constructor
        ///</summary>
        [TestMethod]
        public void Constructor_empty_cells_must_be_set_Test() => CheckBoard(_board);

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod]
        public void Count_is_length_of_cells_Test()
        {
            Assert.AreEqual(Consts.DIMENSIONSQUARE * Consts.DIMENSIONSQUARE, 81);
            Assert.AreEqual(Consts.COUNTCELL, _board.Count);
        }

        /// <summary>
        ///A test for CreateSimpleBoard
        ///</summary>
        [TestMethod]
        public void CreateSimpleBoard_creates_int_with_Test()
        {
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                _board.SetDigit(i, i + 1);
            }
            int[] expected = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var actual = ((Board)_board).CreateSimpleBoard();
            Assert.AreEqual(expected.Length, actual.Length);
            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                var c = Cell.CreateCellFromUniqueID(actual[i]);
                Assert.AreEqual(_board[i].CandidateValue, c.CandidateValue);
                Assert.AreEqual(i, c.ID);
                Assert.AreEqual(_board[i].Digit, expected[i]);
                Assert.AreEqual(_board[i].Digit, c.Digit);
            }

            var recreatedBoard = new Board(actual);
            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                Assert.AreEqual(i, recreatedBoard[i].ID);
                Assert.AreEqual(_board[i].CandidateValue, recreatedBoard[i].CandidateValue);
                Assert.AreEqual(_board[i].Digit, recreatedBoard[i].Digit);
                Assert.AreEqual(_board[i].ID, recreatedBoard[i].ID);
            }
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod]
        public void GetEnumeratorTest_Test()
        {
            IEnumerator<ICell> actual;
            actual = _board.GetEnumerator();
            var i = 0;
            while (actual.MoveNext())
            {
                Assert.AreEqual(actual.Current, _board[i++]);
            }
        }

        /// <summary>
        ///A test for GetHouse
        ///</summary>
        [TestMethod]
        public void GetHouse_House_Col_Test()
        {
            var houseType = HouseType.Col;
            IHouse<Cell> actual;
            for (var idx = 0; idx < Consts.DIMENSIONSQUARE; idx++)
            {
                actual = _board.GetHouse(houseType, idx);
                for (var r = 0; r < Consts.DIMENSIONSQUARE; r++)
                {
                    ICell expected = actual[r];
                    ICell cellRow = _board[idx + (r * Consts.DIMENSIONSQUARE)];
                    Assert.AreEqual(cellRow, expected);
                }
            }
        }

        /// <summary>
        ///A test for GetHouse
        ///</summary>
        [TestMethod]
        public void GetHouse_House_Row_Test()
        {
            var houseType = HouseType.Row;
            IHouse<Cell> actual;
            for (var idx = 0; idx < Consts.DIMENSIONSQUARE; idx++)
            {
                actual = _board.GetHouse(houseType, idx);
                for (var r = 0; r < Consts.DIMENSIONSQUARE; r++)
                {
                    ICell expected = actual[r];
                    ICell cellRow = _board[r + (idx * Consts.DIMENSIONSQUARE)];
                    Assert.AreEqual(cellRow, expected);
                }
            }
        }

        [TestInitialize]
        public void Initialize() => _board = new Board(_solveTechniques);

        /// <summary>
        ///A test for IsComplete
        ///</summary>
        [TestMethod]
        public void IsComplete_board_is_false_at_first_Test()
        {
            bool actual;
            actual = _board.IsComplete();
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for Givens
        ///</summary>
        [TestMethod]
        public void IsGiven_Test()
        {
            var actual = _board.Where(x => x.IsGiven);
            Assert.AreEqual(0, actual.Count());
            _board.SetDigit(1, 1);
            actual = _board.Where(x => x.IsGiven);
            Assert.AreEqual(1, actual.Count());
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod]
        public void ItemTest_Test() => Assert.AreEqual(0, _board[0].ID);

        /// <summary>
        ///A test for SetBoard
        ///</summary>
        [TestMethod]
        public void SetBoard_set_Digit_at_cell_0_Test()
        {
            IBoard<Cell> otherBoard = new Board();
            otherBoard.SetDigit(0, 1);
            Assert.AreEqual(0, _board[0].Digit);
            ((Board)_board).SetBoard(otherBoard);
            Assert.AreEqual(1, _board[0].Digit);
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
        public void SetDigit_LastDigit_produce_an_error_Test()
        {
            _board.SetCellsFromString("003000000456000000789000000000000000000000000000000000000000000000000000000000000");
            // Es ist möglich die 1 oder 2 in Zelle 3 zu setzen. Dies führt aber zu einem Fehler.
            var log = _board.SetDigit(3, 1);
            Assert.IsFalse(log.Successful);
        }

        /// <summary>
        /// A test for SetDigit
        /// </summary>
        [TestMethod]
        public void SetDigit_set_digit_when_only_one_candidate_left_Test()
        {
            _board.SetDigit(0, 1);
            _board.SetDigit(1, 2);
            _board.SetDigit(2, 3);

            _board.SetDigit(9, 4);
            _board.SetDigit(10, 5);
            _board.SetDigit(11, 6);

            _board.SetDigit(18, 7);
            Assert.AreEqual(0, _board[20].Digit);
            Assert.AreEqual(((1 << 7) + (1 << 8)), _board[20].CandidateValue);

            // Now Last
            _board.SetDigit(19, 8);

            Assert.AreEqual(9, _board[20].Digit);
            Assert.AreEqual(0, _board[20].CandidateValue);
        }

        [TestMethod]
        public void SetDigit_with_Alpha_Row_Coordinates_Test()
        {
            ICell firstCell = _board[1];
            Assert.AreEqual(0, firstCell.Digit, "Just to be sure that this value is 0 at first.");
            var result = _board.SetDigit(row: 'a', col: 1, digit: 5);
            Assert.IsTrue(result.Successful);
            firstCell = _board[1];
            Assert.AreEqual(5, firstCell.Digit, "Cell A/1  is same as 0/1 is same as id=1 and must be set to 5");
            result = _board.SetDigit('B', 1, 6);
            Assert.IsTrue(result.Successful);
            firstCell = _board[10];
            Assert.AreEqual(6, firstCell.Digit, "Cell B/1 must be set to 6");
        }

        /// <summary>
        /// A test for SetDigit
        /// </summary>
        [TestMethod]
        public void SetDigit_with_coordinates_Test()
        {
            Assert.AreEqual(0, _board[80].Digit);
            _board.SetDigit(row: 8, col: 8, digit: 9);
            Assert.AreEqual(9, _board[80].Digit);
            Assert.AreEqual(0, _board[80].CandidateValue);
        }

        /// <summary>
        ///A test for SetDigit
        ///</summary>
        [TestMethod]
        public void SetDigitTest3_Test()
        {
            for (var i = 0; i < 8; i++)
            {
                _board.SetDigit(i, i + 1);
                if (i == 0)
                {
                    continue;
                }

                _board.SetDigit((i * 9), 9 - i);
            }
            Assert.AreEqual(9, _board[8].Digit);
            Assert.AreEqual(0, _board[8].CandidateValue);
            Assert.AreEqual(9, _board[72].Digit);
            Assert.AreEqual(0, _board[72].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest4_Test()
        {
            _board.SetDigit(0, 0, 1);
            _board.SetDigit(0, 1, 2);
            _board.SetDigit(0, 2, 3);
            _board.SetDigit(1, 0, 4);
            _board.SetDigit(1, 1, 5);
            _board.SetDigit(1, 3, 7);
            _board.SetDigit(1, 4, 8);
            var result = _board.SetDigit(1, 5, 9);
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(6, _board[11].Digit);
            var block0Value = (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block0Value, _board[18].CandidateValue);
            Assert.AreEqual(block0Value, _board[19].CandidateValue);
            Assert.AreEqual(block0Value, _board[20].CandidateValue);
            var block1r2Value = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);
            Assert.AreEqual(block1r2Value, _board[21].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[22].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[23].CandidateValue);
            _board.StartSolve();
            var block1r2ValueSolve = (1 << 0) | (1 << 1) | (1 << 2);
            Assert.AreEqual(block1r2ValueSolve, _board[21].CandidateValue);
            Assert.AreEqual(block1r2ValueSolve, _board[22].CandidateValue);
            Assert.AreEqual(block1r2ValueSolve, _board[23].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest5_Test()
        {
            _board.SetDigit(1, 4, 1);
            _board.SetDigit(0, 6, 2);
            _board.SetDigit(0, 7, 3);
            var log = _board.SetDigit(0, 8, 4);
            Assert.IsTrue(log.Successful);
            _board.StartSolve();
            var block1r2Value = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block1r2Value, _board[18].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[19].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[20].CandidateValue);
        }

        [TestMethod]
        public void SetDigitTest6_Test()
        {
            var tmpBoard = (Board)_board;
            tmpBoard.SetDigit(4, 1, 1);
            tmpBoard.SetDigit(6, 0, 2);
            tmpBoard.SetDigit(7, 0, 3);
            var result = tmpBoard.SetDigit(8, 0, 4);
            Assert.IsTrue(result.Successful);
            result = _board.StartSolve();
            Assert.IsTrue(result.Successful);
            var block1r2Value = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
            Assert.AreEqual(block1r2Value, _board[2].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[11].CandidateValue);
            Assert.AreEqual(block1r2Value, _board[20].CandidateValue);
        }

        [TestMethod]
        public void Solve_Sudoku_solved_with_3_solvetechniques_and_without_backtracking_Test()
        {
            var board = new Board(_solveTechniques);
            var simpleSudoku = @"030050040
008010500
460000012
070502080
000603000
040109030
250000098
001020600
080060020";

            var lines = simpleSudoku.Split('\n');
            for (var y = 0; y < 9; y++)
            {
                var line = lines[y];

                for (var x = 0; x < 9; x++)
                {
                    var currChar = line[x];
                    if (currChar.Equals('0'))
                    {
                        continue;
                    }

                    var result = board.SetDigit(y, x, Convert.ToInt32(currChar) - 48);
                    Assert.IsTrue(result.Successful);
                }
            }
            Assert.IsFalse(board.IsComplete());
            var sudokuResult = board.StartSolve();
            Assert.IsTrue(board.IsComplete());
            Assert.IsTrue(sudokuResult.Successful);
        }

        /// <summary>
        ///A test for SolvePercent
        ///</summary>
        [TestMethod]
        public void SolvePercent_is_0_at_first_Test() => Assert.AreEqual(0.0, _board.SolvePercent);

        [TestMethod]
        public void SolvePercent_is_greater_than_0_Test()
        {
            _board.SetDigit(0, 1);
            Assert.IsTrue(_board.SolvePercent > 0.0);
        }

        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod]
        public void SolveTest_Test()
        {
            var tmpBoard = (Board)_board;
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

            var result = tmpBoard.SetDigit(0, 0, 6);
            Assert.IsTrue(result.Successful);
            result = _board.StartSolve();
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(2, _board[80].Digit);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToString_is_string_with_every_digit_Test()
        {
            var expected = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            Assert.AreEqual(expected, _board.ToString());
        }

        private static void CheckBoard(IBoard<Cell> target)
        {
            Assert.AreEqual(Consts.COUNTCELL, ((Board)target).Count);
            foreach (ICell cell in target)
            {
                Assert.AreEqual(Consts.BASESTART, cell.CandidateValue);
                Assert.AreEqual(0, cell.Digit);
            }
        }

        private static ASolveTechnique<Cell>[] GetSolveTechniques()
        {
            var st = new ASolveTechnique<Cell>[]
            {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>(),
            };
            return st;
        }
    }
}
