﻿namespace Sudoku.Test.Serialization
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

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) => _solveTechniques =
            [
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>()
            ];

        [TestInitialize]
        public void Initialize() => _board = new Board(_solveTechniques, null);

        [TestMethod]
        public void Serialize_Board_Test()
        {
            _board.SetDigit(0, 1);
            var json = _board.GetJson(new DigitAction
            {
                CellId = 1,
                Digit = 2
            });
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length > 10);
            var tmpBoard = SudokuSerializer.ParseToBoard(json);
            Assert.AreEqual(1, tmpBoard[0].Digit);
        }
    }
}
