namespace DE.Onnen.Sudoku.SolveTechniques
{
    public interface ISolveTechnique<C> where C : ICell
    {
        SolveTechniqueInfo Info { get; }

        bool IsActive { get; }

        void Activate();

        void Deactivate();

        void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult);

        ECellView CellView { get; }
    }
}