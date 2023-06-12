//-----------------------------------------------------------------------
// <copyright file="DigitAction.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace DE.Onnen.Sudoku.Serialization
{
    /// <summary>
    /// Digits set by the user.
    /// </summary>
    [Serializable]
    public record DigitAction
    {
        /// <summary>
        /// Gets or sets Digit-Id of the Cell.
        /// </summary>
        public int CellId { get; init; }

        /// <summary>
        /// Gets or sets the digit that will be set.
        /// </summary>
        public int Digit { get; init; }
    }
}
