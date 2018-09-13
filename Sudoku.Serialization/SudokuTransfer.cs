//-----------------------------------------------------------------------
// <copyright file="SudokuTransfer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace Soduko.Serialization
{
    using System.Collections.ObjectModel;

    // Transfer-Object that includes the last Board constellation and the current actions.
    /// <summary>
    /// Transfer-Object that includes the last Board constellation and the current actions.
    /// </summary>
    public class SudokuTransfer
    {
        // Gets or sets the cell information that represent a board.
        /// <summary>
        /// Gets or sets the cell information that represent a board.
        /// </summary>
        public ReadOnlyCollection<int> Cells { get; set; }

        // Gets or sets the action that set a digit by the user.
        /// <summary>
        /// Gets or sets the action that set a digit by the user.
        /// </summary>
        public ReadOnlyCollection<DigitAction> Action { get; set; }
    }
}