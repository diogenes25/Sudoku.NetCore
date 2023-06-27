using System.Collections.Generic;
using System.Linq;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;

namespace Sudoku.NetCore
{
    public class LastCandidateInHouseTechiques : ASolveTechnique<Cell>
    {
        public SolveTechniqueInfo Info => SolveTechniqueInfo.GetTechniqueInfo(
            caption: "Last Candidate in House",
            descr: "When only "
        );

        /// <inheritdoc />
        public override void SolveBoard(IBoard<Cell> board, SudokuLog sudokuResult)
        {
        }

        /// <inheritdoc />
        public override void SolveHouse(IBoard<Cell> board, IHouse<Cell> house, SudokuLog sudokuResult)
        {
            var singleCandCell = new Dictionary<int, Cell?>();

            foreach (var cell in house.Where(c => c.Digit == 0))
            {
                foreach (var cand in cell.Candidates)
                {
                    if (singleCandCell.ContainsKey(cand))
                    {
                        singleCandCell[cand] = null;
                    }
                    else
                    {
                        singleCandCell.Add(cand, cell);
                    }
                }
            }

            foreach (var cellToSetDigit in singleCandCell.Where(c => c.Value is not null))
            {
                var sresult = sudokuResult.CreateChildResult();
                sresult.EventInfoInResult = new SudokuEvent
                {
                    Value = cellToSetDigit.Key,
                    ChangedCellBase = cellToSetDigit.Value,
                    Action = ECellAction.SetDigitInt,
                    SolveTechnique = "LastCandidate in House",
                };
                cellToSetDigit.Value?.SetDigit(cellToSetDigit.Key, sresult);
            }
        }
    }
}
