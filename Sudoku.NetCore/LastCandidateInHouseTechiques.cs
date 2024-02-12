using System.Collections.Generic;
using System.Linq;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;

namespace Sudoku.NetCore
{
    /// <summary>
    /// If only one candidate is left in a house, it is the last candidate and can be set.
    /// </summary>
    public class LastCandidateInHouseTechiques : ASolveTechnique<Cell>
    {
        /// <inheritdoc />
        public LastCandidateInHouseTechiques() : base(SolveTechniqueInfo.GetTechniqueInfo(
            caption: "Last Candidate in House",
            descr: "When only "
        ))
        {
        }

        /// <summary>
        /// Not Needed because the solve is done in the house.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sudokuResult"></param>
        public override void SolveBoard(IBoard<Cell> board, SudokuLog sudokuResult)
        {
            // Not Needed because the solve is done in the house.
        }

        /// <inheritdoc />
        public override void SolveHouse(IBoard<Cell> board, IHouse<Cell> house, SudokuLog sudokuResult)
        {
            var singleCandCell = new Dictionary<int, Cell?>();

            foreach (var cell in house.Where(c => c.Digit == 0))
            {
                foreach (var cand in cell.Candidates)
                {
                    if (!singleCandCell.TryAdd(cand, cell))
                    {
                        singleCandCell[cand] = null;
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
