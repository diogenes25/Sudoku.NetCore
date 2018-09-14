namespace DE.Onnen.Sudoku.SolveTechniques
{
    public interface ISolveTechnique
    {
        SolveTechniqueInfo Info { get; set; }

        bool IsActive { get; }

        void Activate();

        void Deactivate();


        void SolveHouse(IHouse house, SudokuLog sudokuResult);

        ECellView CellView { get; }
    }
}