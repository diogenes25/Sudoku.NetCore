using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;

namespace Sudoku.KillerSudoku
{
    public class SolveTechniqueKillerSudoku<C> : ASolveTechnique<C> where C : ICell
    {
        public override void SolveBoard(IBoard<C> board, SudokuLog sudokuResult) => throw new NotImplementedException();
        public override void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult) { }
    }
}
