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
    /// <remarks>
    /// Initializes a new instance of the ASolveTechnique class.
    /// </remarks>
    public abstract class ASolveTechnique<C>(SolveTechniqueInfo info) : ISolveTechnique<C> where C : ICell
    {
        /// <inheritdoc />
        public SolveTechniqueInfo Info { get; init; } = info;

        /// <inheritdoc />
        public bool IsActive { get; private set; } = true;

        /// <inheritdoc />
        public void Activate() => IsActive = true;

        /// <inheritdoc />
        public void Deactivate() => IsActive = false;

        /// <inheritdoc/>
        public bool Equals(ISolveTechnique<C> other) => Info.Caption == other?.Info.Caption;

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            if (other is ISolveTechnique<C> solveTechnique)
            {
                return Equals(solveTechnique);
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Info.Caption.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Info.Caption;

        /// <inheritdoc />
        public abstract void SolveBoard(IBoard<C> board, SudokuLog sudokuResult);

        /// <inheritdoc />
        public abstract void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);
    }
}
