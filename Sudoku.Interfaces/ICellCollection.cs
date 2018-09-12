using DE.Onnen.Sudoku;
using System.Collections.Generic;

namespace de.onnen.Sudoku.SudokuExternal
{
    public interface ICellCollection : IEnumerable<ICell>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ICell this[int index]
        {
            get;
        }

        int Count { get; }

        void Clear();
    }
}