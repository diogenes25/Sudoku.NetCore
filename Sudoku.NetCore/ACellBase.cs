using System.Collections.Generic;
using System.ComponentModel;

namespace DE.Onnen.Sudoku
{
    public abstract class ACellBase : ICellBase, INotifyPropertyChanged
    {
        /// <summary>
        /// protected candidateValue to set value without NotifyPropertyChanged-Event.
        /// </summary>
        internal int _candidateValueInternal;

        private int _id;
        private HouseType _hType;

        /// <summary>
        /// Changes in CandidateValue and/or Digit.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public HouseType HType
        {
            get => this._hType;
            protected set => this._hType = value;
        }

        /// <inheritdoc />
        public int ID
        {
            get => this._id;
            protected set => this._id = value;
        }

        /// <inheritdoc />
        public int CandidateValue
        {
            get => this._candidateValueInternal;
            internal set { SetField(ref this._candidateValueInternal, value, nameof(this.CandidateValue)); }
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
            if (obj == null || !(obj is ICellBase))
            {
                return false;
            }

            return this.ID == ((ICellBase)obj).ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}