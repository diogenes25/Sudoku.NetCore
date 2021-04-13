//-----------------------------------------------------------------------
// <copyright file="IBoard.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    using System;

    /// <summary>
    /// Sudoku puzzle.
    /// </summary>
    /// <remarks>
    /// It contains the 81 constituent cells, lined up in 9 rows and 9 columns, with a distinct border around the boxes.
    /// </remarks>
    /// <typeparam name="C">Cell must be derived from ICell</typeparam>
    public interface IBoard<C> : ICellCollection<C>, IEquatable<IBoard<C>>
    where C : ICell
    {
        /// <summary>
        /// Gets the percentage solution progress
        /// </summary>
        /// <returns>Percentage solution progress</returns>
        double SolvePercent { get; }

        /// <summary>
        /// Solve by backtracking (brute force) every Digit.
        /// </summary>
        /// <returns>Log information</returns>
        SudokuLog Backtracking();

        /// <summary>
        /// Returns a specific house.
        /// </summary>
        /// <param name="houseType">Define the HouseType. There are 3 House-Types (Row, Col or Box)</param>
        /// <param name="houseID">House-Id of the House</param>
        /// <returns>House that matches the HouseType an HouseID</returns>
        IHouse<C> GetHouse(HouseType houseType, int houseID);

        /// <summary>
        /// Return true when Sudoku is completed.
        /// </summary>
        /// <returns>true == Board is completely solved.</returns>
        bool IsComplete();

        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="cellID">ID of cell</param>
        /// <param name="digitToSet">Digit that will be set.</param>
        /// <returns>Log-Information of the action that were performed after this digit was set.</returns>
        SudokuLog SetDigit(int cellID, int digitToSet);

        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">g-Information of the action that were performed during the solve process.</param>
        /// <returns>true == No Errors while trying to solve. It does not mean the Sudoku was solved completely</returns>
        bool Solve(SudokuLog sudokuResult);
    }

    /// <summary>
    /// Constant values.
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// Initial value of every candidate as a  Bitmask.
        /// </summary>
        /// <remarks>
        /// In normal sudoku = (9 Bit) = 2^9 = 511
        /// </remarks>
        public const int BASESTART = (1 << DIMENSIONSQUARE) - 1;

        /// <summary>
        /// Number of cells total.
        /// </summary>
        /// <returns>In a normal sudoku it should be 81 ((3*3) * (3*3))</returns>
        public const int COUNTCELL = DIMENSIONSQUARE * DIMENSIONSQUARE;

        /// <summary>
        /// Edge length of a box.<br />
        /// </summary>
        /// <remarks>
        /// In a normal sudoku t is 3.<br />
        /// Ultimately, all other terms are based on this value.
        /// </remarks>
        public const int DIMENSION = 3;

        /// <summary>
        /// Total edge length of the sudoku.<br />
        /// </summary>
        /// <remarks>
        /// In normal sudoku = 9 (3*3).
        /// </remarks>
        public const int DIMENSIONSQUARE = DIMENSION * DIMENSION;

        /// <summary>
        /// Number of possible candidates.
        /// </summary>
        public const double SOLVEPERCENTBASE = DIMENSIONSQUARE * DIMENSIONSQUARE * DIMENSIONSQUARE;
    }
}
