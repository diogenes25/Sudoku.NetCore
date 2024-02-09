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

    /// <summary>
    /// Provides methods to serialize and deserialize Sudoku boards.
    /// </summary>
    public static class SudokuSerializer
    {
        /// <summary>
        /// Serializes the specified Sudoku board to JSON format.
        /// </summary>
        /// <param name="board">The Sudoku board to serialize.</param>
        /// <param name="digActions">The digit actions to include in the serialization.</param>
        /// <returns>The JSON representation of the Sudoku board.</returns>
        public static string GetJson(this Board board, params DigitAction[] digActions)
        {
            var transfer = new SudokuDto
            {
                Cells = new List<int>(board.CreateSimpleBoard()),
                Action = new List<DigitAction>(digActions),
            };

            return JsonSerializer.Serialize(transfer);
        }

        /// <summary>
        /// Deserializes the specified JSON string to a Sudoku board.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="solveTechniques">The solve techniques to use for solving the board.</param>
        /// <returns>The deserialized Sudoku board.</returns>
        public static Board ParseToBoard(string json, params ASolveTechnique<Cell>[] solveTechniques)
        {
            var transfer = JsonSerializer.Deserialize<SudokuDto>(json);
            var board = new Board(transfer.Cells, solveTechniques);
            foreach (var boardAction in transfer.Action)
            {
                board.SetDigit(boardAction.CellId, boardAction.Digit);
            }
            return board;
        }
    }
}
