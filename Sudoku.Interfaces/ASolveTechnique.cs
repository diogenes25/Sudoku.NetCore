namespace DE.Onnen.Sudoku.SolveTechniques
{
    public enum ECellView
    {
        OnlyHouse,
        GlobalView,
    }

    public abstract class ASolveTechnique<C> : ISolveTechnique<C> where C : ICell
    {
        private bool isActive = true;

        protected ASolveTechnique()
        {
            this.Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Set Caption", descr: "Need Description");
        }

        #region ISolveTechnic Members

        public abstract void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);

        public SolveTechniqueInfo Info { get; protected set; }

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