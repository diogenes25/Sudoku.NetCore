using System;

namespace DE.Onnen.Sudoku
{
    public delegate void CellEventHandler(SudokuEvent eventInfo);

    public class SudokuEvent : EventArgs
    {
        public ICellBase ChangedCellBase { get; set; }

        public CellAction Action { get; set; }

        private int _value;

        public int Value { set { this._value = value; } }

        public String SolveTechnik;

        public override string ToString()
        {
            return String.Format("{0}, A:{1}, Val:{2}, T:{3}", this.ChangedCellBase, this.Action, this._value, this.SolveTechnik);
        }
    }

    public enum CellAction
    {
        SetDigitInt = 1,
        RemPoss = 2,
    }
}