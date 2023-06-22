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
        protected ASolveTechnique() => Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Set Caption", descr: "Need Description");

        /// <inheritdoc />
//        public virtual ECellView CellView => ECellView.OnlyHouse;

        /// <inheritdoc />
        public SolveTechniqueInfo Info { get; protected set; }

        /// <inheritdoc />
        public bool IsActive { get; private set; } = true;

        /// <inheritdoc />
        public void Activate() => IsActive = true;

        /// <inheritdoc />
        public void Deactivate() => IsActive = false;

        public abstract void SolveBoard(IBoard<C> board, SudokuLog sudokuResult);

        /// <inheritdoc />
        public abstract void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);
    }
}
