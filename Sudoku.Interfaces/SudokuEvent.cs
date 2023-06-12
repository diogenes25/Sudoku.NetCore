namespace DE.Onnen.Sudoku
{
    public enum CellAction
    {
        SetDigitInt = 1,
        RemoveCandidate = 2,
    }

    public class SudokuEvent : System.EventArgs
    {
        #region Public Properties

        public CellAction Action { get; set; }

        public IHasCandidates ChangedCellBase { get; set; }

        public string SolveTechnik { private get; set; }

        public int Value { private get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0}, A:{1}, Val:{2}, T:{3}", ChangedCellBase, Action, Value, SolveTechnik);
        }

        #endregion Public Methods
    }
}
