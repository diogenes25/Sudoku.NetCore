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
        #region Public Constructors

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
                CellID = -1;
                Digit = -1;
            }
            else
            {
                CellID = cell.ID;
                Digit = cell.Digit;
            }

            SudokuResults = sudokuResult;
            if (board == null)
            {
                Percent = 0;
            }
            else
            {
                BoardInt = new System.Collections.ObjectModel.ReadOnlyCollection<int>(board.CreateSimpleBoard());
                Percent = board.SolvePercent;
            }
        }

        #endregion Public Constructors

        #region Public Properties

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
        /// Gets percentage solution progress
        /// </summary>
        public double Percent { get; private set; }

        /// <summary>
        /// Gets Last result.
        /// </summary>
        public SudokuLog SudokuResults { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Info about the work, that had been done.
        /// </summary>
        /// <returns>Information of this step.</returns>
        public override string ToString() => string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                "Cell({0}) [{1}{2}] {3} {4}%",
                 CellID,
                 (char)(int)((CellID / Consts.DIMENSIONSQUARE) + 65),
                 (CellID % Consts.DIMENSIONSQUARE) + 1,
                 Digit,
                 string.Format("{0:0.00}", Percent));

        #endregion Public Methods
    }
}
