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

        protected AHasCandidates(int id, HouseType ht)
        {
            ID = id;
            HType = ht;
            Clear();
        }

        /// <summary>
        /// Changes in CandidateValue and/or Digit.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public ReadOnlyCollection<int> Candidates
        {
            get
            {
                var retInt = new List<int>();
                for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
                {
                    if (((1 << i) & CandidateValue) > 0)
                    {
                        retInt.Add(i + 1);
                    }
                }
                return retInt.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public int CandidateValue
        {
            get => _candidateValueInternal;
            internal set => SetField(ref _candidateValueInternal, value, nameof(CandidateValue));
        }

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

        public void Clear() => _candidateValueInternal = Consts.BASESTART;

        public override bool Equals(object other)
        {
            if (other == null || other is not IHasCandidates)
            {
                return false;
            }

            return Equals((IHasCandidates)other);
        }

        public bool Equals(AHasCandidates other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals((IHasCandidates)other);
        }

        public bool Equals(IHasCandidates other)
        {
            if (other == null)
            {
                return false;
            }

            return (ID == other.ID) && HType == other.HType;
        }

        public override int GetHashCode() => ID + (((int)HType) * 100);

        /// <inheritdoc />
        public bool RemoveCandidate(int candidateToRemove, SudokuLog sudokuResult)
        {
            var tmpSudokuResult = sudokuResult;
            tmpSudokuResult ??= new SudokuLog();

            if (candidateToRemove < 1 || candidateToRemove > Consts.DIMENSIONSQUARE || (CandidateValue & (1 << (candidateToRemove - 1))) == 0)
            {
                return false;
            }

            CandidateValue -= (1 << (candidateToRemove - 1));

            var eventInfoInResult = new SudokuEvent
            {
                ChangedCellBase = this,
                Action = ECellAction.RemoveCandidate,
                SolveTechnik = "SetDigit",
                Value = candidateToRemove,
            };

            var nakeResult = tmpSudokuResult.CreateChildResult();
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
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                // convert Digit to BitMask (example: 4 => (1 << 4 ) = (Byte)00001000
                // Whenn BitMask & CandidateValue still Candidate Value then must the Digit the last Candidate (or bit)
                // Example false: BitMask = 00001000 (4) and there are 2 Candidate (4 and 5) left = 00011000
                // 00001000 & 00011000 = 00001000 != 00011000 (Candidates)
                // Excample true: only Candidate 4 ist left (00001000)
                // 00001000 & 00001000 = 00001000 == 00001000 (Candidates)
                if (((1 << i) & CandidateValue) == CandidateValue)
                {
                    var sresult = sudokuResult.CreateChildResult();
                    sresult.EventInfoInResult = new SudokuEvent
                    {
                        Value = i + 1,
                        ChangedCellBase = this,
                        Action = ECellAction.RemoveCandidate,
                        SolveTechnik = "LastCandidate",
                    };

                    return SetDigit(i + 1, sresult);
                }
            }
            return false;
        }

        internal abstract bool SetDigit(int digit, SudokuLog sudokuResult);

        /// <summary>
        /// Cell-Value (CandidateValue and/or Digit) changed.
        /// </summary>
        /// <param name="propertyName">Digit or CadidateValue</param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
    }
}
