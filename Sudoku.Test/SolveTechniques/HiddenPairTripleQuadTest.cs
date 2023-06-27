using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
    [TestClass]
    public class HiddenPairTripleQuadTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;
        private IBoard<Cell> _board;

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
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                Assert.AreEqual((i + 1), _board[i].Digit);
            }
        }

        [TestInitialize]
        public void Initialize() => _board = new Board(_solveTechniques, null);

        private static ASolveTechnique<Cell>[] GetSolveTechniques() => new ASolveTechnique<Cell>[] {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>()
            };

        /// <summary>
        /// Cell[0] and Cell[1] must have ONLY candidate 7 and 8.
        /// </summary>
        /// <remarks>
        /// Setze folgendes Sudoku
        /// !!0000000 (here is a HiddenPair with the candidates 7 and 8)
        /// 000000078
        /// 000780000
        /// 007000000
        /// 008000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// 000000000
        /// </remarks>
        [TestMethod]
        public void HiddenPairTripleQuadTest_in_Row_Test()
        {
            IBoard<Cell> board = new Board();

            board.SetCellsFromString("000000000000000078000780000007000000008000000000000000000000000000000000000000000");
            Assert.IsTrue(board[0].Candidates.Contains(1));
            Assert.IsTrue(board[1].Candidates.Contains(1));
            Assert.AreEqual(9, board[0].Candidates.Count);
            Assert.AreEqual(9, board[1].Candidates.Count);

            board.StartSolve();
            // No changes after StartSolve()
            Assert.IsTrue(board[0].Candidates.Contains(1));
            Assert.IsTrue(board[1].Candidates.Contains(1));
            Assert.AreEqual(9, board[0].Candidates.Count);
            Assert.AreEqual(9, board[1].Candidates.Count);

            var hidden = new HiddenPairTripleQuad<Cell>();

            // Check Box:0
            hidden.SolveHouse(board, board.GetHouse(EHouseType.Box, 0), new SudokuLog());
            Assert.IsFalse(board[0].Candidates.Contains(1));
            Assert.IsFalse(board[1].Candidates.Contains(1));
            Assert.AreEqual(2, board[0].Candidates.Count, "Cell[0].Candidates must be reduced to 7 and 8");
            Assert.AreEqual(2, board[1].Candidates.Count, "Cell[1].Candidates must be reduced to 7 and 8");
        }

        [TestMethod]
        public void Doc()
        {
            var board = new Board();

            board.SetCellsFromString("000000000000000078000780000007000000008000000000000000000000000000000000000000000");
            // Show init Board
            System.Console.WriteLine("Given Board with candidates:");
            System.Console.WriteLine(board.MatrixWithCandidates());

            // Create solvetechnique
            var hidden = new HiddenPairTripleQuad<Cell>();

            // Start Solve with first row.
            hidden.SolveHouse(board, board.GetHouse(EHouseType.Box, 0), new SudokuLog());

            // Init Board
            System.Console.WriteLine("Board with candidates 7 and 8 left");
            System.Console.WriteLine(board.MatrixWithCandidates());
        }
    }
}
