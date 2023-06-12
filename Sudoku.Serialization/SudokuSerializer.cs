//-----------------------------------------------------------------------
// <copyright file="SudokuSerializer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.Serialization
{
    using System.Collections.Generic;
    using System.Text.Json;
    using DE.Onnen.Sudoku;
    using DE.Onnen.Sudoku.SolveTechniques;

    public static class SudokuSerializer
    {
        #region Public Methods

        public static string GetJson(this Board board, params DigitAction[] digActions)
        {
            var transfer = new SudokuTransfer
            {
                Cells = new List<int>(board.CreateSimpleBoard()),
                Action = new List<DigitAction>(digActions),
            };

            return JsonSerializer.Serialize(transfer);
        }

        public static Board ParseToBoard(string json, params ASolveTechnique<Cell>[] solveTechniques)
        {
            var transfer = JsonSerializer.Deserialize<SudokuTransfer>(json);
            var board = new Board(transfer.Cells, solveTechniques);
            foreach (var boardAction in transfer.Action)
            {
                board.SetDigit(boardAction.CellId, boardAction.Digit);
            }
            return board;
        }

        #endregion Public Methods
    }
}
