namespace DE.Onnen.Sudoku
{
    public class SudokuEvent : System.EventArgs
    {
        public IHasCandidates ChangedCellBase { get; set; }
        public CellAction Action { get; set; }
        public int Value { private get; set; }
        public System.String SolveTechnik { private get; set; }

        public override string ToString()
        {
            return System.String.Format("{0}, A:{1}, Val:{2}, T:{3}", this.ChangedCellBase, this.Action, this.Value, this.SolveTechnik);
        }
    }

    public enum CellAction
    {
        SetDigitInt = 1,
        RemoveCandidate = 2,
    }
}