//-----------------------------------------------------------------------
// <copyright file="SudokuTransfer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.Serialization
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Transfer-Object that includes the last Board constellation and the current actions.
    /// </summary>
    public class SudokuTransfer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuTransfer" /> class.
        /// </summary>
        public SudokuTransfer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuTransfer" /> class.
        /// </summary>
        /// <param name="board">Board to convert</param>
        public SudokuTransfer(Board board) => Cells = new ReadOnlyCollection<int>(board.CreateSimpleBoard());

        /// <summary>
        /// Gets or sets the action that set a digit by the user.
        /// </summary>
        public ReadOnlyCollection<DigitAction> Action { get; set; }

        /// <summary>
        /// Gets or sets the cell information that represent a board.
        /// </summary>
        public ReadOnlyCollection<int> Cells { get; set; }
    }
}
