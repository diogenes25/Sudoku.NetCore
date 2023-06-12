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
        #region Internal Fields

        internal int _digit;

        internal House<Cell>[] _fieldcontainters = new House<Cell>[3];

        #endregion Internal Fields

        #region Internal Constructors

        internal Cell(int id) : base(id, HouseType.Cell)
        {
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <inheritdoc />
        public new int CandidateValue
        {
            get => base.CandidateValue;
            internal set
            {
                base.CandidateValue = value;
                if (base.CandidateValue > 0 && Digit > 0 && value > 0)
                {
                    _digit = 0;
                }
            }
        }

        /// <inheritdoc />
        public int Digit
        {
            get => _digit;
            internal set
            {
                if (value == 0)
                {
                    _digit = 0;
                    CandidateValue = Consts.BASESTART;
                }
                else
                {
                    if (_digit == value)
                    {
                        return;
                    }

                    if (value > 0 && value <= Consts.DIMENSIONSQUARE && _digit < 1 && (CandidateValue & (1 << (value - 1))) == (1 << (value - 1)))
                    {
                        if (SetField(ref _digit, value, nameof(Digit)))
                        {
                            CandidateValue = 0;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Digit {value} is in {this} not possible");
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool IsGiven { get; internal set; }

        #endregion Public Properties

        #region Public Methods

        public static Cell CreateCellFromUniqueID(int x)
        {
            var mask = ((1 << 7) - 1);
            var tmpCandidates = 0;
            var tmpDigit = 0;
            int tmpId;
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
            var retVAl = new Cell(tmpId)
            {
                _candidateValueInternal = tmpCandidates,
                _digit = tmpDigit
            };
            return retVAl;
        }

        public static int CreateUniqueID(int id, int digit) => ((digit << 7) + id) * -1;

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ICell))
            {
                return false;
            }

            return ID == ((ICell)obj).ID;
        }

        public bool Equals(ICell othercell)
        {
            if (othercell == null)
            {
                return false;
            }
            return ID == othercell.ID;
        }

        public bool Equals(Cell other) => Equals((ICell)other);

        public override int GetHashCode() => base.GetHashCode();

        public int GetUniqueID()
        {
            if (Digit == 0)
            {
                return (CandidateValue << 7) + ID;
            }
            else
            {
                return CreateUniqueID(ID, Digit);
            }
        }

        /// <inheritdoc />
        public SudokuLog SetDigit(int digitToSet)
        {
            var sudokuLog = new SudokuLog();
            SetDigit(digitToSet, sudokuLog);
            return sudokuLog;
        }

        /// <summary>
        /// Every Cell-information.
        /// </summary>
        /// <returns>String that contains every cell-information.</returns>
        public override string ToString() => HType + "(" + ID + ") [" + ((char)(int)((ID / Consts.DIMENSIONSQUARE) + 65)) + "" + ((ID % Consts.DIMENSIONSQUARE) + 1) + "] " + _digit;

        #endregion Public Methods

        #region Internal Methods

        internal override bool SetDigit(int digitFromOutside, SudokuLog sudokuResult)
        {
            if (_digit == digitFromOutside)
            {
                return false;
            }

            var result = sudokuResult.CreateChildResult();
            result.EventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = CellAction.SetDigitInt,
                SolveTechnik = "None",
                Value = digitFromOutside,
            };

            try
            {
                Digit = digitFromOutside;
            }
            catch (Exception e)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = e.Message;
                return true;
            }

            for (var i = 0; i < 3; i++)
            {
                if (_fieldcontainters[i] == null)
                {
                    continue;
                }
                _fieldcontainters[i].SetDigit(digitFromOutside, sudokuResult);
                if (!sudokuResult.Successful)
                {
                    sudokuResult.ErrorMessage = "Digit " + digitFromOutside + " is in Cell (FieldContainer) " + ID + " not possible";
                    return true;
                }
            }

            return true;
        }

        #endregion Internal Methods
    }
}
