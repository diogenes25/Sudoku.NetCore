using System.Collections.Generic;

namespace DE.Onnen.Sudoku
{
    public interface ICellCollection<T> : IEnumerable<T> where T : ICell
    {
        // Get a specific cells of the board.
        /// <summary>
        /// Get a specific cells of the board.
        /// </summary>
        /// <param name="index">Id of the cell.</param>
        /// <returns>Cell with the specific ID</returns>
        T this[int index] { get; }

        // Gets the number of Cells
        /// <summary>
        /// Gets the number of Cells
        /// </summary>
        /// <returns>Number of cells.</returns>
        int Count { get; }

        /// <summary>
        /// Clears every Cell information and sets the Board to start values.
        /// </summary>
        void Clear();
    }
}