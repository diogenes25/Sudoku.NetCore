using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.SolveTechniques;
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
            var target = Board.PureBoard().AddSolveTechnique(new LockedCandidates<Cell>());
            var log = target.Backtracking();
            Assert.IsTrue(log.Successful);
            Assert.IsTrue(target.IsComplete());
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual((i + 1), target[i].Digit);
            }
        }

        /// <summary>
        /// Candidate 5 must be in box0 so 5 must be removed as an cadidate in cell 33,34 and 35
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// 123000000
        /// 000500000
        /// 000000!!! Candidate 5 must be in box:0 so it must be removed from Cell 24, 25 and 26
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void LockedCandidates_in_Box_Test()
        {
            IBoard<Cell> board = Board.PureBoard();

            board.SetCellsFromString("123000000000500000000000000000000000000000000000000000000000000000000000000000000");
            // System.Console.WriteLine(board.MatrixWithCandidates());

            Assert.IsTrue(board[24].Candidates.Contains(5));
            Assert.IsTrue(board[25].Candidates.Contains(5));
            Assert.IsTrue(board[26].Candidates.Contains(5));
            Assert.AreEqual(9, board[24].Candidates.Count);
            Assert.AreEqual(9, board[25].Candidates.Count);
            Assert.AreEqual(9, board[26].Candidates.Count);

            // System.Console.WriteLine(board.MatrixWithCandidates());
            var locked = new LockedCandidates<Cell>();

            // Check Box:0
            locked.SolveHouse(board, board.GetHouse(EHouseType.Box, 0), new SudokuLog());
            // System.Console.WriteLine(board.MatrixWithCandidates());
            Assert.IsFalse(board[24].Candidates.Contains(5), "Cell[24].Candidates must not contains 5");
            Assert.IsFalse(board[25].Candidates.Contains(5), "Cell[25].Candidates must not contains 5");
            Assert.AreEqual(8, board[24].Candidates.Count, "Cell[24].Candidates must not contains 5");
            Assert.AreEqual(8, board[25].Candidates.Count, "Cell[25].Candidates must not contains 5");
        }

        /// <summary>
        /// Candidate 5 must be in box0 so 5 must be removed as an cadidate in cell 33,34 and 35
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// 000456789
        /// !!!000000 Remove 1,2 and 3
        /// !!!000000 Remove 1,2 and 3
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void LockedCandidates_in_Row_Test()
        {
            var board = Board.PureBoard();

            board.SetCellsFromString("000456789000000000000000000000000000000000000000000000000000000000000000000000000");

            Assert.IsTrue(board[9].Candidates.Contains(1));
            Assert.IsTrue(board[10].Candidates.Contains(1));
            Assert.IsTrue(board[27].Candidates.Contains(1));
            Assert.AreEqual(9, board[9].Candidates.Count);
            Assert.AreEqual(9, board[10].Candidates.Count);
            Assert.AreEqual(9, board[27].Candidates.Count);

            new LockedCandidates<Cell>().SolveHouse(board, board.GetHouse(EHouseType.Row, 0), new SudokuLog());

            Assert.IsFalse(board[9].Candidates.Contains(1), "Cell[24].Candidates must not contains 5");
            Assert.IsFalse(board[10].Candidates.Contains(2), "Cell[25].Candidates must not contains 5");
            Assert.AreEqual(6, board[9].Candidates.Count, "Cell[9] has 6 Candidates left. 1,2 and 3 are removed");
            Assert.AreEqual(6, board[19].Candidates.Count, "Cell[27] has 6 Candidates left. 1,2 and 3 are removed");
        }
    }
}
