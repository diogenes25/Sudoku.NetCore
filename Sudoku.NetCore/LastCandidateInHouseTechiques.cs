using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;

namespace Sudoku.NetCore
{
    public class LastCandidateInHouseTechiques : ISolveTechnique<Cell>
    {
        public SolveTechniqueInfo Info => SolveTechniqueInfo.GetTechniqueInfo(
            caption: "Last Candidate in House",
            descr:  "When only "
        );
        public bool IsActive { get; set; } = true;

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        public void SolveBoard(IBoard<Cell> board, SudokuLog sudokuResult) { }
        public void SolveHouse(IBoard<Cell> board, IHouse<Cell> house, SudokuLog sudokuResult) {
            
            var single = 0;
            var doubleSav = 0;
            var doubleBefore = 0;

            foreach (var cell in house)
            {
                doubleSav |= cell.CandidateValue & doubleBefore;
                doubleBefore = cell.CandidateValue;
                single ^= cell.CandidateValue;
            }

            var bit = ~doubleSav & single;
            if (bit == 0)
            {
                return;
            }

            // Check every possible Digit
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                if (((1 << i) & bit) > 0)
                {
                    var cellToSetDigit = house.Where(v => (v.CandidateValue & (1 << i)) > 0).FirstOrDefault();
                    if (cellToSetDigit is not null)
                    {
                        var sresult = sudokuResult.CreateChildResult();
                        sresult.EventInfoInResult = new SudokuEvent
                        {
                            Value = i + 1,
                            ChangedCellBase = cellToSetDigit,
                            Action = ECellAction.SetDigitInt,
                            SolveTechnique = "LastCandidate in House",

                        };
                        cellToSetDigit.SetDigit((i + 1), sresult);
                    }
                }
            }
        }

    }
}
