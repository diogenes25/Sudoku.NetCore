//-----------------------------------------------------------------------
// <copyright file="SudokuEvent.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Represents the action performed on a Sudoku cell.
    /// </summary>
    public enum ECellAction
    {
        /// <summary>
        /// Sets a digit in the cell.
        /// </summary>
        SetDigitInt = 1,

        /// <summary>
        /// Removes a candidate from the cell.
        /// </summary>
        RemoveCandidate = 2,
    }

    /// <summary>
    /// Represents an event that occurs in a Sudoku puzzle.
    /// </summary>
    public class SudokuEvent : System.EventArgs
    {
        /// <summary>
        /// Gets or sets the action associated with the event.
        /// </summary>
        public ECellAction Action { get; set; }

        /// <summary>
        /// Gets or sets the cell that has been changed.
        /// </summary>
        public IHasCandidates ChangedCellBase { get; set; }

        /// <summary>
        /// Gets or sets the solve technique used to make the change.
        /// </summary>
        /// <remarks>
        /// This property is private and can only be accessed within the class.
        /// </remarks>
        public string SolveTechnique { private get; set; }

        /// <summary>
        /// Gets or sets the value associated with the change.
        /// </summary>
        /// <remarks>
        /// This property is private and can only be accessed within the class.
        /// </remarks>
        public int Value { private get; set; }

        /// <summary>
        /// Returns a string representation of the SudokuEvent object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => string.Format("{0}, A:{1}, Val:{2}, T:{3}", ChangedCellBase, Action, Value, SolveTechnique);
    }
}
