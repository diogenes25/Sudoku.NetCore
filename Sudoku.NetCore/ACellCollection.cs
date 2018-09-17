using System;
using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    public class ACellCollection<C> : ICellCollection<C> where C : Cell
    {
        protected C[] _cells;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public C this[int index] => this._cells[index];

        #region ICollection<ICell> Members

        public bool Contains(Cell item)
        {
            return this._cells.Contains(item);
        }

        public int Count
        {
            get { return this._cells.Count(); }
        }

        #endregion ICollection<ICell> Members

        #region IEnumerable<ICell> Members

        public IEnumerator<C> GetEnumerator()
        {
            return this._cells.Select(x => (C)x).GetEnumerator();
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