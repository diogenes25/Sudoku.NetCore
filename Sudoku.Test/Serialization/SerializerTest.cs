namespace Sudoku.Test.Serialization
{
    using DE.Onnen.Sudoku;
    using DE.Onnen.Sudoku.Serialization;
    using DE.Onnen.Sudoku.SolveTechniques;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializerTest
    {
        private static ASolveTechnique<Cell>[] _solveTechniques;
        private Board _board;

        [TestInitialize]
        public void Initialize()
        {
            this._board = new Board(_solveTechniques);
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _solveTechniques = new ASolveTechnique<Cell>[]
                {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>()
                };
        }

        [TestMethod]
        public void Serialize_Board_Test()
        {
            this._board.SetDigit(0, 1);
            string json = this._board.GetJson(new DigitAction(1, 2));
            Board tmpBoard = SudokuSerializer.ParseToBoard(json);
            Assert.AreEqual(1, tmpBoard[0].Digit);
        }
    }
}