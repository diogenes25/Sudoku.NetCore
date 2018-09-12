/// <summary>
/// 
/// </summary>
namespace Sudoku.Test.Serialization
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using DE.Onnen.Sudoku;
    using DE.Onnen.Sudoku.SolveTechniques;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Soduko.Serialization;

    [TestClass]
    public class SerializerTest
    {
        private static ASolveTechnique[] _solveTechniques;
        private Board _board;

        [TestInitialize]
        public void Initialize()
        {
            this._board = new Board(_solveTechniques);
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _solveTechniques = new ASolveTechnique[]
                {
                new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad(),
                new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates(),
                new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad()
                };
        }

        [TestMethod]
        public void Serialize_Board_Test()
        {
            this._board.SetDigit(0, 1);
            string json = this._board.GetJson(1, 2);
            Board tmpBoard = SudokuSerializer.ParseToBoard(json);
            Assert.AreEqual(1, tmpBoard[0].Digit);
        }
    }
}
