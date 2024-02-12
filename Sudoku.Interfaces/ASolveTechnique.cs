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
    /// <typeparam name="T">Type of Cell</typeparam>
    /// <remarks>
    /// Initializes a new instance of the ASolveTechnique class.
    /// </remarks>
    public abstract class ASolveTechnique<T>(SolveTechniqueInfo info) : ISolveTechnique<T> where T : ICell
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
        public bool Equals(ISolveTechnique<T> other) => Info.Caption == other?.Info.Caption;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ISolveTechnique<T> solveTechnique)
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
        public abstract void SolveBoard(IBoard<T> board, SudokuLog sudokuResult);

        /// <inheritdoc />
        public abstract void SolveHouse(IBoard<T> board, IHouse<T> house, SudokuLog sudokuResult);
    }
}
