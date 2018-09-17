//-----------------------------------------------------------------------
// <copyright file="ASolveTechnique.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku.SolveTechniques
{
    /// <summary>
    /// A Simple core implementation of the Interface ISolveTechnique.
    /// </summary>
    /// <typeparam name="C">Type of Cell</typeparam>
    public abstract class ASolveTechnique<C> : ISolveTechnique<C> where C : ICell
    {
        /// <summary>
        /// Initializes a new instance of the ASolveTechnique class.
        /// </summary>
        protected ASolveTechnique()
        {
            this.Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Set Caption", descr: "Need Description");
        }

        #region ISolveTechnic Members

        /// <inheritdoc />
        public bool IsActive { get; private set; } = true;

        /// <inheritdoc />
        public SolveTechniqueInfo Info { get; protected set; }

        /// <inheritdoc />
        public virtual ECellView CellView => ECellView.OnlyHouse;

        /// <inheritdoc />
        public void Activate() => this.IsActive = true;

        /// <inheritdoc />
        public void Deactivate() => this.IsActive = false;

        /// <inheritdoc />
        public abstract void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);

        #endregion ISolveTechnic Members
    }
}