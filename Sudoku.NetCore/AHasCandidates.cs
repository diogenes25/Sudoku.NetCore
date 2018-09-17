using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DE.Onnen.Sudoku
{
    public abstract class AHasCandidates : IHasCandidates, INotifyPropertyChanged, System.IEquatable<AHasCandidates>
    {
        /// <summary>
        /// protected candidateValue to set value without NotifyPropertyChanged-Event.
        /// </summary>
        internal int _candidateValueInternal;

        /// <summary>
        /// Changes in CandidateValue and/or Digit.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public HouseType HType
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public int ID
        {
            get;
            private set;
        }

        protected AHasCandidates(int id, HouseType ht)
        {
            this.ID = id;
            this.HType = ht;
            this.Clear();
        }

        /// <inheritdoc />
        public int CandidateValue
        {
            get => this._candidateValueInternal;
            internal set { SetField(ref this._candidateValueInternal, value, nameof(this.CandidateValue)); }
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

        internal abstract bool SetDigit(int digit, SudokuLog sudokuResult);

        /// <summary>
        /// Cell-Value (CandidateValue and/or Digit) changed.
        /// </summary>
        /// <param name="propertyName">Digit or CadidateValue</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set new PropertyValue and fire ProperyChanged-Event when old value differs from new value.
        /// </summary>
        /// <typeparam name="T">Propertytype</typeparam>
        /// <param name="field">Property</param>
        /// <param name="value">new value</param>
        /// <param name="propertyName">Propertyname</param>
        /// <returns>true = Value changed</returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IHasCandidates))
            {
                return false;
            }

            return Equals((IHasCandidates)obj);
        }

        public bool Equals(AHasCandidates obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals((IHasCandidates)obj);
        }

        public bool Equals(IHasCandidates obj)
        {
            if (obj == null)
            {
                return false;
            }

            return (this.ID == obj.ID) && this.HType == obj.HType;
        }

        public override int GetHashCode()
        {
            return this.ID + (((int)this.HType) * 100);
        }

        public void Clear()
        {
            this._candidateValueInternal = Consts.BaseStart;
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
                Action = CellAction.RemoveCandidate,
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
                        Action = CellAction.RemoveCandidate,
                        SolveTechnik = "LastCandidate",
                    };

                    return SetDigit(i + 1, sresult);
                }
            }
            return false;
        }
    }
}