//-----------------------------------------------------------------------
// <copyright file="ICellCollection.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Contains cells.
    /// </summary>
    /// <remarks>A Board has Cells and Houses contains Cells as well.</remarks>
    /// <typeparam name="T">A Cell must be derived from ICell</typeparam>
    public interface ICellCollection<out T> : System.Collections.Generic.IEnumerable<T> where T : ICell
    {
        /// <summary>
        /// Gets the number of Cells
        /// </summary>
        /// <returns>Number of cells.</returns>
        int Count { get; }

        /// <summary>
        /// Get a specific cells of the board.
        /// </summary>
        /// <param name="index">Id of the cell.</param>
        /// <returns>Cell with the specific ID</returns>
        T this[int index] { get; }

        /// <summary>
        /// Clears every Cell information and sets the Board to start values.
        /// </summary>
        void Clear();
    }
}