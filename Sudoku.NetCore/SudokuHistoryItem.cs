//-----------------------------------------------------------------------
// <copyright file="SudokuHistoryItem.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// A clone of the current board including some extra info.
    /// </summary>
    public class SudokuHistoryItem
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="SudokuHistoryItem" /> class.
        /// </summary>
        /// <remarks>Create a small clone of the board.</remarks>
        /// <param name="board">Current board</param>
        /// <param name="cell">Last changed Cell</param>
        /// <param name="sudokuResult">associated log entry</param>
        public SudokuHistoryItem(Board board, ICell cell, SudokuLog sudokuResult)
        {
            if (cell == null)
            {
                this.CellID = -1;
                this.Digit = -1;
            }
            else
            {
                this.CellID = cell.ID;
                this.Digit = cell.Digit;
            }

            this.SudokuResults = sudokuResult;
            if (board == null)
            {
                this.Percent = 0;
            }
            else
            {
                this.BoardInt = new System.Collections.ObjectModel.ReadOnlyCollection<int>(board.CreateSimpleBoard());
                this.Percent = board.SolvePercent;
            }
        }

        /// <summary>
        /// Gets a collection of integer that representing the board.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<int> BoardInt { get; private set; }

        /// <summary>
        /// Gets a CellID of the cell that was set before this HistoryItem was created.
        /// </summary>
        public int CellID { get; private set; }

        /// <summary>
        /// Gets Digit that was set before this HistoryItem was created.
        /// </summary>
        public int Digit { get; private set; }

        /// <summary>
        /// Gets Last result.
        /// </summary>
        public SudokuLog SudokuResults { get; private set; }

        /// <summary>
        /// Gets percentage solution progress
        /// </summary>
        public double Percent { get; private set; }

        /// <summary>
        /// Info about the work, that had been done.
        /// </summary>
        /// <returns>Information of this step.</returns>
        public override string ToString()
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                "Cell({0}) [{1}{2}] {3} {4}%",
                 this.CellID,
                 (char)(int)((this.CellID / Consts.DimensionSquare) + 65),
                 (this.CellID % Consts.DimensionSquare) + 1,
                 this.Digit,
                 string.Format("{0:0.00}", this.Percent));
        }
    }
}