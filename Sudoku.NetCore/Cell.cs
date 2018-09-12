using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sudoku.Test")]

namespace DE.Onnen.Sudoku
{
    /// <inheritdoc cref="ICell"/>
    [DebuggerDisplay("Cell-ID {id} {digit} / {CandidateValue}")]
    public class Cell : ACellBase, ICell, IEquatable<Cell>
    {
        private int _digit;

        internal House[] _fieldcontainters = new House[3];

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
                        if (SetField(ref this._digit, value, "Digit"))
                        {
                            this.CandidateValue = 0;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Digit " + value + " is in " + this + " not possible");
                    }
                }
            }
        }

        internal Cell(int id)
        {
            this.HType = HouseType.Cell;
            this.ID = id;
            this.CandidateValue = Consts.BaseStart;
        }

  
        public bool Equals(ICell othercell)
        {
            if (othercell == null )
            {
                return false;
            }
            return this.ID == othercell.ID;
        }

        /// <inheritdoc />
        public bool RemoveCandidate(int candidateToRemove, SudokuLog sudokuResult)
        {
            SudokuLog tmpSudokuResult = sudokuResult;
            if (tmpSudokuResult == null)
            {
                tmpSudokuResult = new SudokuLog();
            }

            if (candidateToRemove < 1 || candidateToRemove > Consts.DimensionSquare || (this.CandidateValue & (1 << (candidateToRemove - 1))) == 0)
            {
                return false;
            }

            this.CandidateValue -= (1 << (candidateToRemove - 1));

            SudokuEvent eventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = CellAction.RemPoss,
                SolveTechnik = "SetDigit",
                Value = candidateToRemove,
            };

            SudokuLog nakeResult = tmpSudokuResult.CreateChildResult();
            nakeResult.EventInfoInResult = eventInfoInResult;

            CheckLastDigit(tmpSudokuResult);

            if (!tmpSudokuResult.Successful)
            {
                nakeResult.Successful = false;
                nakeResult.ErrorMessage = "RemoveCandidateValue";
                return true;
            }

            return true;
        }

        /// <summary>
        /// Check if there is only one candidate left.
        /// </summary>
        /// <param name="sudokuResult">Log </param>
        /// <returns>true = Only one candidate was left and will be set</returns>
        internal bool CheckLastDigit(SudokuLog sudokuResult)
        {
            // Check every possible Digit
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                // convert Digit to BitMask (example: 4 => (1 << 4 ) = (Byte)00001000
                // Whenn BitMask & CandidateValue still Candidate Value then must the Digit the last Candidate (or bit)
                // Example false: BitMask = 00001000 (4) and there are 2 Candidate (4 and 5) left = 00011000
                // 00001000 & 00011000 = 00001000 != 00011000 (Candidates)
                // Excample true: only Candidate 4 ist left (00001000)
                // 00001000 & 00001000 = 00001000 == 00001000 (Candidates)
                if (((1 << i) & this.CandidateValue) == this.CandidateValue)
                {
                    SudokuLog sresult = sudokuResult.CreateChildResult();
                    sresult.EventInfoInResult = new SudokuEvent
                    {
                        Value = i + 1,
                        ChangedCellBase = this,
                        Action = CellAction.RemPoss,
                        SolveTechnik = "LastCandidate",
                    };

                    return SetDigit(i + 1, sresult);
                }
            }
            return false;
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

        /// <summary>
        /// Uniqe hashcode of the cell.
        /// </summary>
        /// <remarks>
        /// When digit not set: negative value. First 9 Bits represents the candidates, next bits are the Cell-ID.<br />
        /// When digit set: Positive value. First 9 Bits is the digit, next bits are the Cell-ID
        /// </remarks>
        /// <returns>Uniqe int with every information.</returns>
        public override int GetHashCode()
        {
            return (this._digit == 0) ? (this.CandidateValue + ((1 << Consts.Dimension) + this.ID) * -1) : (this._digit + ((1 << Consts.Dimension) + this.ID));
        }

        public bool Equals(Cell other)
        {
            return this.Equals((ICell)other);
        }

        /// <inheritdoc />
        public ReadOnlyCollection<int> Candidates
        {
            get
            {
                List<int> retInt = new List<int>();
                for (int i = 0; i < Consts.DimensionSquare; i++)
                {
                    if (((1 << i) & this.CandidateValue) > 0)
                    {
                        retInt.Add(i + 1);
                    }
                }
                return retInt.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public bool IsGiven
        {
            get;
            set;
        }
    }
}