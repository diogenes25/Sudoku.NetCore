using System;

namespace DE.Onnen.Sudoku
{
    public delegate void CellEventHandler(SudokuEvent eventInfo);

    public class SudokuEvent : EventArgs
    {
        private int _value;
        private String _solveTechnik;

        public ICellBase ChangedCellBase { get; set; }
        public CellAction Action { get; set; }
        public int Value { private get { return this._value; } set { this._value = value; } }
        public String SolveTechnik { private get { return this._solveTechnik; } set { this._solveTechnik = value; } }

        public override string ToString()
        {
            return String.Format("{0}, A:{1}, Val:{2}, T:{3}", this.ChangedCellBase, this.Action, this.Value, this.SolveTechnik);
        }
    }

    public enum CellAction
    {
        SetDigitInt = 1,
        RemPoss = 2,
    }
}