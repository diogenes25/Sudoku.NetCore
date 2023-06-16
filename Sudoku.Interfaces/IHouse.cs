//-----------------------------------------------------------------------
// <copyright file="IHouse.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Type of house.
    /// </summary>
    public enum EHouseType
    {
        /// <summary>
        /// A Row
        /// </summary>
        Row = 0,

        /// <summary>
        /// A Column
        /// </summary>
        Col = 1,

        /// <summary>
        /// Box (or Block)
        /// </summary>
        Box = 2,

        /// <summary>
        /// Singe cell
        /// </summary>
        Cell = 3,
    }

    /// <summary>
    /// A group of 9 cells.
    /// </summary>
    /// <remarks>
    /// Each cell must contain a different digit in the solution.
    /// In standard sudoku, a house can be a row, a column or a box. There are 27 houses in a standard sudoku grid.
    /// </remarks>
    /// <typeparam name="C">Cell must be derived from ICell</typeparam>
    public interface IHouse<C> : IHasCandidates, ICellCollection<C> where C : ICell
    {
        /// <summary>
        /// Checks a House that every cell in this House is set and each digit is only st once.
        /// </summary>
        /// <returns>true == Every digit is set (correctly).</returns>
        bool IsComplete();
    }
}
