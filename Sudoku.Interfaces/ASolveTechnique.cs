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
        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the ASolveTechnique class.
        /// </summary>
        protected ASolveTechnique()
        {
            Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Set Caption", descr: "Need Description");
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <inheritdoc />
        public virtual ECellView CellView => ECellView.OnlyHouse;

        /// <inheritdoc />
        public SolveTechniqueInfo Info { get; protected set; }

        /// <inheritdoc />
        public bool IsActive { get; private set; } = true;

        #endregion Public Properties

        #region Public Methods

        /// <inheritdoc />
        public void Activate() => IsActive = true;

        /// <inheritdoc />
        public void Deactivate() => IsActive = false;

        /// <inheritdoc />
        public abstract void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);

        #endregion Public Methods
    }
}
