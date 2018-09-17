//-----------------------------------------------------------------------
// <copyright file="SudokuSerializer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace Soduko.Serialization
{
    using DE.Onnen.Sudoku;
    using DE.Onnen.Sudoku.SolveTechniques;
    using Newtonsoft.Json;
    using System.Collections.ObjectModel;

    public static class SudokuSerializer
    {
        public static string GetJson(this Board board, params DigitAction[] digActions)
        {
            SudokuTransfer transfer = new SudokuTransfer
            {
                Cells = new ReadOnlyCollection<int>(board.CreateSimpleBoard()),
                Action = new ReadOnlyCollection<DigitAction>(digActions),
            };

            return JsonConvert.SerializeObject(transfer, Formatting.Indented);
        }

        public static Board ParseToBoard(string json, params ASolveTechnique<Cell>[] solveTechniques)
        {
            SudokuTransfer transfer = JsonConvert.DeserializeObject<SudokuTransfer>(json);
            Board board = new Board(transfer.Cells, solveTechniques);
            foreach (DigitAction boardAction in transfer.Action)
            {
                board.SetDigit(boardAction.CellId, boardAction.Digit);
            }
            return board;
        }
    }
}