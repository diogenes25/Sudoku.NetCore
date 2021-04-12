//-----------------------------------------------------------------------
// <copyright file="SudokuSerializer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.Serialization
{
    using System.Collections.ObjectModel;
    using DE.Onnen.Sudoku;
    using DE.Onnen.Sudoku.SolveTechniques;
    using Newtonsoft.Json;

    public static class SudokuSerializer
    {
        public static string GetJson(this Board board, params DigitAction[] digActions)
        {
            var transfer = new SudokuTransfer
            {
                Cells = new ReadOnlyCollection<int>(board.CreateSimpleBoard()),
                Action = new ReadOnlyCollection<DigitAction>(digActions),
            };

            return JsonConvert.SerializeObject(transfer, Formatting.Indented);
        }

        public static Board ParseToBoard(string json, params ASolveTechnique<Cell>[] solveTechniques)
        {
            var transfer = JsonConvert.DeserializeObject<SudokuTransfer>(json);
            var board = new Board(transfer.Cells, solveTechniques);
            foreach (var boardAction in transfer.Action)
            {
                board.SetDigit(boardAction.CellId, boardAction.Digit);
            }
            return board;
        }
    }
}
