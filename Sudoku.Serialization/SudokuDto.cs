﻿//-----------------------------------------------------------------------
// <copyright file="SudokuDto.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Transfer-Object that includes the last Board constellation and the current actions.
    /// </summary>
    [Serializable]
    public record SudokuDto
    {
        /// <summary>
        /// Gets or sets the action that set a digit by the user.
        /// </summary>
        public IEnumerable<DigitAction> Action { get; init; }

        /// <summary>
        /// Gets or sets the cell information that represent a board.
        /// </summary>
        public IEnumerable<int> Cells { get; init; }
    }
}
