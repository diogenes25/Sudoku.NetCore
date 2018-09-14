using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    public class House : ACellBase, IHouse
    {
        private readonly ICell[] cells;

        /// <summary>
        /// true = some cells
        /// </summary>
        internal bool ReCheck { set; get; }

        /// <summary>
        /// Every Cell inside this House has a Digit.
        /// </summary>
        public bool IsComplete()
        {
            int retval = 0;
            int checkDigit = 0;
            foreach (Cell c in this.cells)
            {
                retval |= c.CandidateValue;
                checkDigit ^= (1 << (c.Digit - 1));
                if ((c.CandidateValue == 0 && c.Digit == 0) || (c.CandidateValue > 0 && c.Digit != 0))
                {
                    throw new System.ArgumentException($"Cell{c.ID} in House {ID} has an invalid status: Digit: {c.Digit} / Candidate: {c.CandidateValue}");
                }
            }
            if (retval == 0 && checkDigit != Consts.BaseStart)
            {
                throw new System.ArgumentException("House is invalid");
            }

            return retval == 0 && checkDigit == Consts.BaseStart;
        }

        internal House(ICell[] cells, HouseType containerType, int containerIdx)
        {
            this.CandidateValue = Consts.BaseStart;
            this.cells = cells;
            this.HType = containerType;
            this.ID = containerIdx;
            this.ReCheck = false;
            foreach (ICell c in cells)
            {
                c.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }
        }

        private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Digit"))
            {
                this.ReCheck = true;
            }
        }

        /// <summary>
        /// A Digit in one of his peers is set.
        /// </summary>
        /// <remarks>
        /// Check if digit is possible in this house.
        /// </remarks>
        /// <param name="digit"></param>
        /// <param name="sudokuResult"></param>
        /// <returns></returns>
        internal override bool SetDigit(int digit, SudokuLog sudokuResult)
        {
            SudokuEvent sudokuEvent = new SudokuEvent
            {
                Value = digit,
                SolveTechnik = "SetDigit",
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt
            };
            bool ok = true;
            int newBaseValue = Consts.BaseStart;
            HashSet<int> m = new HashSet<int>();
            foreach (ICell cell in this.cells)
            {
                if (cell.Digit > 0)
                {
                    if (m.Contains(cell.Digit))
                    {
                        ok = false;
                        break;
                    }
                    else
                    {
                        m.Add(cell.Digit);
                        newBaseValue -= (1 << (cell.Digit - 1));
                    }
                }
            }

            if (!ok)
            {
                SudokuLog resultError = sudokuResult.CreateChildResult();
                resultError.EventInfoInResult = sudokuEvent;
                resultError.Successful = false;
                resultError.ErrorMessage = "Digit " + digit + " is in CellContainer not possible";
                return true;
            }

            this._candidateValueInternal = newBaseValue;

            SudokuLog result = sudokuResult.CreateChildResult();
            result.EventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt,
                SolveTechnik = "None",
                Value = digit,
            };

            foreach (Cell cell in this.cells)
            {
                cell.RemoveCandidate(digit, result);
            }

            return true;
        }

        public override string ToString()
        {
            return this.HType + "(" + this.ID + ") " + this.CandidateValue;
        }

        public ICell this[int index]
        {
            get { return this.cells[index]; }
        }

        #region IEnumerable<ICell> Members

        public IEnumerator<ICell> GetEnumerator()
        {
            return this.cells.Select(x => (ICell)x).GetEnumerator();
        }

        #endregion IEnumerable<ICell> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.cells.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}