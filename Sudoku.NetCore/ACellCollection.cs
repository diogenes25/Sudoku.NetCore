using de.onnen.Sudoku.SudokuExternal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    public class ACellCollection : ICellCollection
    {
        protected Cell[] _cells;

        /// <summary>
        /// Reset Board.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this._cells[i].Digit = 0;
            }
        }

        #region IList<ICell> Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ICell this[int index] => this._cells[index];

        #endregion IList<ICell> Members

        #region ICollection<ICell> Members

      

        public bool Contains(ICell item)
        {
            return this._cells.Contains(item);
        }

    

        public int Count
        {
            get { return this._cells.Count(); }
        }

        

        #endregion ICollection<ICell> Members

        #region IEnumerable<ICell> Members

        public IEnumerator<ICell> GetEnumerator()
        {
            return this._cells.Select(x => (ICell)x).GetEnumerator();
        }

        #endregion IEnumerable<ICell> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._cells.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}