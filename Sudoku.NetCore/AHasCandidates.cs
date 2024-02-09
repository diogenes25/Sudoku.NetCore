using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Abstract base class for objects that have candidates.
    /// </summary>
    public abstract class AHasCandidates : IHasCandidates, INotifyPropertyChanged //, System.IEquatable<AHasCandidates>
    {
        /// <summary>
        /// protected candidateValue to set value without NotifyPropertyChanged-Event.
        /// </summary>
        internal int _candidateValueInternal = Consts.BASESTART;

        /// <summary>
        /// Initializes a new instance of the <see cref="AHasCandidates"/> class.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <param name="ht">The house type of the object.</param>
        protected AHasCandidates(int id, EHouseType ht)
        {
            ID = id;
            HType = ht;
        }

        /// <summary>
        /// Changes in CandidateValue and/or Digit.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
        public EHouseType HType { get; private set; }

        /// <inheritdoc />
        public int ID { get; private set; }

        /// <summary>
        /// Set the candidates to all possible Numbers.
        /// </summary>
        /// <see cref="Consts.BASESTART"/>
        public virtual void Clear() => _candidateValueInternal = Consts.BASESTART;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? other)
        {
            if (other == null || other is not IHasCandidates)
            {
                return false;
            }

            return Equals((IHasCandidates)other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(IHasCandidates? other)
        {
            if (other == null)
            {
                return false;
            }

            return (ID == other.ID) && HType == other.HType;
        }

        /// <summary>
        /// Calculates the hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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
                SolveTechnique = "SetDigit",
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
        /// <param name="sudokuResult">The log.</param>
        /// <returns>true if only one candidate was left and will be set; otherwise, false.</returns>
        public bool CheckLastDigit(SudokuLog sudokuResult)
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
                        SolveTechnique = "LastCandidate in Cell",
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
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Set new property value and fire PropertyChanged event when old value differs from new value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The backing field of the property.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>true if the value changed; otherwise, false.</returns>
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
