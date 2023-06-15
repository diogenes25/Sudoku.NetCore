namespace DE.Onnen.Sudoku
{
    public enum ECellAction
    {
        SetDigitInt = 1,
        RemoveCandidate = 2,
    }

    public class SudokuEvent : System.EventArgs
    {
        public ECellAction Action { get; set; }

        public IHasCandidates ChangedCellBase { get; set; }

        public string SolveTechnique { private get; set; }

        public int Value { private get; set; }

        public override string ToString() => string.Format("{0}, A:{1}, Val:{2}, T:{3}", ChangedCellBase, Action, Value, SolveTechnique);
    }
}
