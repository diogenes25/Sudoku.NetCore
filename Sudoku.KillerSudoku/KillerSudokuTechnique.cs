using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;

namespace DE.Onnen.Sudoku.SolveTechniques.KillerSudoku
{
    public class KillerSudokuTEchnique<C> : ASolveTechnique<C> where C : ICell
    {
        private readonly List<KillerHouse> _houses = new();
        private int _id = 0;

        public KillerHouse AddHouse(int sum, int x, int y)
        {
            var retHouse = new KillerHouse(sum, x, y, _id++);
            _houses.Add(retHouse);
            return retHouse;
        }

        public override void SolveBoard(IBoard<C> board, SudokuLog sudokuResult) => throw new NotImplementedException();

        public override void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult)
        {
        }

        public KillerHouse GetHouse(int id)
        {
            return _houses[id];
        }
    }
}
