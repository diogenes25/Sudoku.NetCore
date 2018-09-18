//-----------------------------------------------------------------------
// <copyright file="DigitAction.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.Serialization
{
    /// <summary>
    /// Digits set by the user.
    /// </summary>
    public class DigitAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitAction" /> class.
        /// </summary>
        public DigitAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitAction" /> class.
        /// </summary>
        /// <param name="cellId">ID of the Cell</param>
        /// <param name="digit">Digit to set</param>
        public DigitAction(int cellId, int digit)
        {
            this.CellId = cellId;
            this.Digit = digit;
        }

        /// <summary>
        /// Gets or sets Digit-Id of the Cell.
        /// </summary>
        public int CellId { get; set; }

        /// <summary>
        /// Gets or sets the digit that will be set.
        /// </summary>
        public int Digit { get; set; }
    }
}