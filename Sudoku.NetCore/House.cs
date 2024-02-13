using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    /// <inheritdoc />
    public class House : AHasCandidates, IHouse<Cell>
    {
        private readonly Cell[] _cells;

        internal House(Cell[] cells, EHouseType containerType, int containerIdx) : base(containerIdx, containerType)
        {
            _cells = cells;
            ReCheck = false;
            foreach (var c in cells)
            {
                c.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }
        }

        /// <summary>
        /// Number of Cells.
        /// </summary>
        public int Count => _cells.Length;

        /// <summary>
        /// true = some cells inside this house where been changed and another solve-routine should be done.
        /// </summary>
        internal bool ReCheck { set; get; }

        /// <summary>
        /// Cell[index] of this house.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A spezific cell</returns>
        Cell ICellCollection<Cell>.this[int index] => _cells[index];

        /// <summary>
        /// Cell[index] of this house.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A spezific cell</returns>
        public Cell this[int index] => _cells[index];

        /// <summary>
        /// Enumerator of the house.
        /// </summary>
        /// <returns>IEnumerator<ICell> of the cell of the house.</ICell></returns>
        public IEnumerator<ICell> GetEnumerator() => _cells.Select(x => (ICell)x).GetEnumerator();

        /// <summary>
        /// Enumerator of the house.
        /// </summary>
        /// <returns>IEnumerator<ICell> of the cell of the house.</ICell></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cells.GetEnumerator();

        /// <summary>
        /// Enumerator of the house.
        /// </summary>
        /// <returns>IEnumerator<ICell> of the cell of the house.</ICell></returns>
        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => _cells.ToList().GetEnumerator();

        /// <summary>
        /// When every Cell inside this House has a Digit, then this house is 'Complete'.
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

        /// <summary>
        /// String of the House.
        /// </summary>
        /// <returns>String with Couse-ID and CandidateValue.</returns>
        public override string ToString() => $"{HType} ({ID}) {CandidateValue}";

        /// <summary>
        /// A Digit in one of his peers is set.
        /// </summary>
        /// <remarks>
        /// Check if digit is possible in this house and remove this digit as an candidate from the included cells.
        /// </remarks>
        /// <param name="digit"></param>
        /// <param name="sudokuResult"></param>
        /// <returns></returns>
        internal override bool SetDigit(int digit, SudokuLog sudokuResult)
        {
            var sudokuEvent = new SudokuEvent
            {
                Value = digit,
                SolveTechnique = "SetDigit",
                ChangedCellBase = this,
                Action = ECellAction.SetDigitInt
            };
            var newCalculatedCandidateValue = Consts.BASESTART;
            var digitsFoundInHouse = new HashSet<int>();
            foreach (var cell in _cells.Where(c => c.Digit > 0))
            {
                if (digitsFoundInHouse.Contains(cell.Digit))
                {
                    var resultError = sudokuResult.CreateChildResult();
                    resultError.EventInfoInResult = sudokuEvent;
                    resultError.Successful = false;
                    resultError.ErrorMessage = $"Digit {digit} allready set in another Cell is in CellContainer {HType} {ID}.";
                    return true;
                }
                else
                {
                    ReCheck = true;
                    digitsFoundInHouse.Add(cell.Digit);
                    newCalculatedCandidateValue -= (1 << (cell.Digit - 1));
                }
            }

            _candidateValueInternal = newCalculatedCandidateValue;

            var result = sudokuResult.CreateChildResult();
            result.EventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = ECellAction.SetDigitInt,
                SolveTechnique = "None",
                Value = digit,
            };

            foreach (var cell in _cells)
            {
                cell.RemoveCandidate(digit, result);
            }

            return true;
        }

        private void Cell_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Digit" || e.PropertyName == "CandidateValue")
            {
                ReCheck = true;
            }
        }
    }
}
