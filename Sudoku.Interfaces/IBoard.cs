//-----------------------------------------------------------------------
// <copyright file="IBoard.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sudoku puzzle.
    /// <remarks>
    /// It contains the 81 constituent cells, lined up in 9 rows and 9 columns, with a distinct border around the boxes.
    /// </remarks>
    /// </summary>
    public interface IBoard : IEnumerable<ICell>, IEquatable<IBoard>
    {
        // Gets the percentage solution progress
        /// <summary>
        /// Gets the percentage solution progress
        /// </summary>
        /// <returns>Percentage solution progress</returns>
        double SolvePercent { get; }
 
        // Gets the number of Cells
        /// <summary>
        /// Gets the number of Cells
        /// </summary>
        /// <returns>Number of cells.</returns>
        int Count { get; }

        // Get a specific cells of the board.
        /// <summary>
        /// Get a specific cells of the board.
        /// </summary>
        /// <param name="index">Id of the cell.</param>
        /// <returns>Cell with the specific ID</returns>
        ICell this[int index] { get; }

        // Returns a specific house.
        /// <summary>
        /// Returns a specific house.
        /// </summary>
        /// <param name="houseType">Define the HouseType. There are 3 House-Types (Row, Col or Box)</param>
        /// <param name="houseID">House-Id of the House</param>
        /// <returns>House that matches the HouseType an HouseID</returns>
        IHouse GetHouse(HouseType houseType, int houseID);

        // Return true when Sudoku is completed.
        /// <summary>
        /// Return true when Sudoku is completed.
        /// </summary>
        /// <returns>true == Board is completely solved.</returns>
        bool IsComplete();

        // Set a digit at cell.
        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="cellID">ID of cell</param>
        /// <param name="digitToSet">Digit that will be set.</param>
        /// <returns>Log-Information of the action that were performed after this digit was set.</returns>
        SudokuLog SetDigit(int cellID, int digitToSet);

        // Solves Sudoku with SolveTechniques (no Backtracking).
        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">g-Information of the action that were performed during the solve process.</param>
        /// <returns>true == No Errors while trying to solve. It does not mean the Sudoku was solved completely</returns>
        bool Solve(SudokuLog sudokuResult);

        // Solve by backtracking (brute force) every Digit.
        /// <summary>
        /// Solve by backtracking (brute force) every Digit.
        /// </summary>
        /// <returns>Log information</returns>
        SudokuLog Backtracking();  
        
        /// <summary>
        /// Clears every Cell information and sets the Board to start values.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Constant values.
    /// </summary>
    public static class Consts
    {
        // Edge length of a box.<br />
        /// <summary>
        /// Edge length of a box.<br />
        /// </summary>
        /// <remarks>
        /// In a normal sudoku t is 3.<br />
        /// Ultimately, all other terms are based on this value.
        /// </remarks>
        public static readonly int Dimension = 3;

        // Total edge length of the sudoku.<br />
        /// <summary>
        /// Total edge length of the sudoku.<br />
        /// </summary>
        /// <remarks>
        /// In normal sudoku = 9 (3*3).
        /// </remarks>
        public static readonly int DimensionSquare = Dimension * Dimension;

        // Initial value of every candidate as a  Bitmask.
        /// <summary>
        /// Initial value of every candidate as a  Bitmask.
        /// </summary>
        /// <remarks>
        /// In normal sudoku = (9 Bit) = 2^9 = 511
        /// </remarks>
        public static readonly int BaseStart = (1 << Consts.DimensionSquare) - 1;

        // Number of cells total.
        /// <summary>
        /// Number of cells total.
        /// </summary>
        /// <returns>In a normal sudoku it should be 81 ((3*3) * (3*3))</returns>
        public static readonly int CountCell = Consts.DimensionSquare * Consts.DimensionSquare;
    }
}