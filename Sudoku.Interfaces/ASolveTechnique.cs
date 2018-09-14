namespace DE.Onnen.Sudoku.SolveTechniques
{
    public enum ECellView
    {
        OnlyHouse,
        GlobalView,
    }

    public abstract class ASolveTechnique : ISolveTechnique
    {
        private bool isActive = true;
        protected IBoard board;

        protected ASolveTechnique()
        {
            this.Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Set Caption", descr: "Need Description");
        }

        public void SetBoard(IBoard board)
        {
            this.board = board;
        }

        #region ISolveTechnic Members

        public abstract void SolveHouse(IHouse house, SudokuLog sudokuResult);

        public SolveTechniqueInfo Info
        {
            get;
            set;
        }

        public void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        public bool IsActive
        {
            get { return this.isActive; }
            private set
            {
                this.isActive = value;
            }
        }

        public virtual ECellView CellView
        {
            get { return ECellView.GlobalView; }
        }

        #endregion ISolveTechnic Members
    }


}