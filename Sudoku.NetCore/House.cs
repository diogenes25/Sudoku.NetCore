using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    public class House<C> : AHasCandidates, IHouse<C>
    where C : ICell
    {
        #region Private Fields

        private readonly C[] _cells;

        #endregion Private Fields

        #region Internal Constructors

        internal House(C[] cells, HouseType containerType, int containerIdx) : base(containerIdx, containerType)
        {
            _cells = cells;
            ReCheck = false;
            foreach (var c in cells)
            {
                c.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }
        }

        #endregion Internal Constructors

        #region Public Properties

        public int Count => _cells.Length;

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// true = some cells
        /// </summary>
        internal bool ReCheck { set; get; }

        #endregion Internal Properties

        #region Public Indexers

        C ICellCollection<C>.this[int index] => _cells[index];

        public ICell this[int index] => _cells[index];

        #endregion Public Indexers

        #region Public Methods

        public IEnumerator<ICell> GetEnumerator() => _cells.Select(x => (ICell)x).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cells.GetEnumerator();

        IEnumerator<C> IEnumerable<C>.GetEnumerator() => _cells.ToList().GetEnumerator();

        /// <summary>
        /// Every Cell inside this House has a Digit.
        /// </summary>
        public bool IsComplete()
        {
            var retval = 0;
            var checkDigit = 0;
            foreach (var c in _cells)
            {
                retval |= c.CandidateValue;
                checkDigit ^= (1 << (c.Digit - 1));
                if ((c.CandidateValue == 0 && c.Digit == 0) || (c.CandidateValue > 0 && c.Digit != 0))
                {
                    throw new System.ArgumentException($"Cell{c.ID} in House {ID} has an invalid status: Digit: {c.Digit} / Candidate: {c.CandidateValue}");
                }
            }
            if (retval == 0 && checkDigit != Consts.BASESTART)
            {
                throw new System.ArgumentException("House is invalid");
            }

            return retval == 0 && checkDigit == Consts.BASESTART;
        }

        public override string ToString() => $"{HType} ({ID}) {CandidateValue}";

        #endregion Public Methods

        #region Internal Methods

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
            var sudokuEvent = new SudokuEvent
            {
                Value = digit,
                SolveTechnik = "SetDigit",
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt
            };
            var ok = true;
            var newBaseValue = Consts.BASESTART;
            var m = new HashSet<int>();
            foreach (var cell in _cells)
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
                var resultError = sudokuResult.CreateChildResult();
                resultError.EventInfoInResult = sudokuEvent;
                resultError.Successful = false;
                resultError.ErrorMessage = "Digit " + digit + " is in CellContainer not possible";
                return true;
            }

            _candidateValueInternal = newBaseValue;

            var result = sudokuResult.CreateChildResult();
            result.EventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt,
                SolveTechnik = "None",
                Value = digit,
            };

            foreach (var cell in _cells)
            {
                cell.RemoveCandidate(digit, result);
            }

            return true;
        }

        #endregion Internal Methods

        #region Private Methods

        private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Digit"))
            {
                ReCheck = true;
            }
        }

        #endregion Private Methods
    }
}
