//-----------------------------------------------------------------------
// <copyright file="SudokuTransfer.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace Soduko.Serialization
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Transfer-Object that includes the last Board constellation and the current actions.
    /// </summary>
    public class SudokuTransfer
    {
        /// <summary>
        /// Gets or sets the cell information that represent a board.
        /// </summary>
        public ReadOnlyCollection<int> Cells { get; set; }

        /// <summary>
        /// Gets or sets the action that set a digit by the user.
        /// </summary>
        public ReadOnlyCollection<DigitAction> Action { get; set; }
    }
}