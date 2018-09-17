using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sudoku.Test")]

namespace DE.Onnen.Sudoku
{
    /// <inheritdoc cref="ICell"/>
    [DebuggerDisplay("Cell-ID {ID} {_digit} / {CandidateValue}")]
    public class Cell : AHasCandidates, ICell, IEquatable<Cell>
    {
        internal int _digit;

        internal House<Cell>[] _fieldcontainters = new House<Cell>[3];

        /// <inheritdoc />
        public new int CandidateValue
        {
            get { return base.CandidateValue; }
            internal set
            {
                base.CandidateValue = value;
                if (base.CandidateValue > 0 && this.Digit > 0 && value > 0)
                {
                    this._digit = 0;
                }
            }
        }

        /// <inheritdoc />
        public int Digit
        {
            get { return this._digit; }
            internal set
            {
                if (value == 0)
                {
                    this._digit = 0;
                    this.CandidateValue = Consts.BaseStart;
                }
                else
                {
                    if (this._digit == value)
                    {
                        return;
                    }

                    if (value > 0 && value <= Consts.DimensionSquare && this._digit < 1 && (this.CandidateValue & (1 << (value - 1))) == (1 << (value - 1)))
                    {
                        if (SetField(ref this._digit, value, nameof(this.Digit)))
                        {
                            this.CandidateValue = 0;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Digit {value} is in {this} not possible");
                    }
                }
            }
        }

        internal Cell(int id) : base(id, HouseType.Cell)
        {
        }

        public static Cell CreateCellFromUniqueID(int x)
        {
            int mask = ((1 << 7) - 1);
            int tmpId = 0;
            int tmpCandidates = 0;
            int tmpDigit = 0;
            if (x < 0)
            {
                tmpId = (x * -1) & mask;
                tmpDigit = (x * -1) >> 7;
            }
            else
            {
                tmpId = x & mask;
                tmpCandidates = x >> 7;
            }
            Cell retVAl = new Cell(tmpId)
            {
                _candidateValueInternal = tmpCandidates,
                _digit = tmpDigit
            };
            return retVAl;
        }

        public int GetUniqueID()
        {
            int retVal = 0;
            if (this.Digit == 0)
            {
                retVal = (this.CandidateValue << 7) + this.ID;
            }
            else
            {
                retVal = CreateUniqueID(this.ID, this.Digit);
            }
            return retVal;
        }

        public static int CreateUniqueID(int id, int digit)
        {
            return ((digit << 7) + id) * -1;
        }

        /// <inheritdoc />
        public SudokuLog SetDigit(int digitToSet)
        {
            SudokuLog sudokuLog = new SudokuLog();
            SetDigit(digitToSet, sudokuLog);
            return sudokuLog;
        }

        internal override bool SetDigit(int digitFromOutside, SudokuLog sudokuResult)
        {
            if (this._digit == digitFromOutside)
            {
                return false;
            }

            SudokuLog result = sudokuResult.CreateChildResult();
            result.EventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt,
                SolveTechnik = "None",
                Value = digitFromOutside,
            };

            try
            {
                this.Digit = digitFromOutside;
            }
            catch (Exception e)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = e.Message;
                return true;
            }

            for (int i = 0; i < 3; i++)
            {
                if (this._fieldcontainters[i] == null)
                {
                    continue;
                }
                this._fieldcontainters[i].SetDigit(digitFromOutside, sudokuResult);
                if (!sudokuResult.Successful)
                {
                    sudokuResult.ErrorMessage = "Digit " + digitFromOutside + " is in Cell (FieldContainer) " + this.ID + " not possible";
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Every Cell-information.
        /// </summary>
        /// <returns>String that contains every cell-information.</returns>
        public override string ToString()
        {
            return this.HType + "(" + this.ID + ") [" + ((char)(int)((this.ID / Consts.DimensionSquare) + 65)) + "" + ((this.ID % Consts.DimensionSquare) + 1) + "] " + this._digit;
        }

        /// <inheritdoc />
        public bool IsGiven { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ICell))
            {
                return false;
            }

            return this.ID == ((ICell)obj).ID;
        }

        public bool Equals(ICell othercell)
        {
            if (othercell == null)
            {
                return false;
            }
            return this.ID == othercell.ID;
        }

        public bool Equals(Cell other)
        {
            return this.Equals((ICell)other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}