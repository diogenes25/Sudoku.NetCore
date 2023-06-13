using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku.SolveTechniques
{
    /// <summary>
    /// Hidden Pair / Triple/ Quad
    /// </summary>
    /// <remarks>
    /// This technique is very similar to naked subsets,
    /// but instead of affecting other cells with the same row,
    /// column or block, candidates are eliminated from the cells that hold the subset.
    /// If there are N cells, with N candidates between them that don't appear elsewhere in the same row,
    /// column or block, then any other candidates for those cells can be eliminated.
    /// </remarks>
    public class HiddenPairTripleQuad<C> : ASolveTechnique<C> where C : ICell
    {
        public HiddenPairTripleQuad() => Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Hidden TwinTripleQuad", descr: "This technique is very similar to naked subsets, but instead of affecting other cells with the same row, column or block, candidates are eliminated from the cells that hold the subset. If there are N cells, with N candidates between them that don't appear elsewhere in the same row, column or block, then any other candidates for those cells can be eliminated.");

        public override ECellView CellView => ECellView.OnlyHouse;

        public override void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult)
        {
            // Digit 1:n Cell (contains Digit)
            // Key = Digit - Value = Alle Zellen die dieses Enthalten
            var digitInCell = new Dictionary<int, List<ICell>>();

            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                if (house[i].Digit == 0)
                {
                    var posDigit = house[i].Candidates;
                    foreach (var num in posDigit)
                    {
                        if (!digitInCell.ContainsKey(num))
                        {
                            digitInCell.Add(num, new List<ICell>());
                        }
                        digitInCell[num].Add(house[i]);
                    }
                }
            }

            // Key = Anzahl der Zellen Value = Liste der Zellen Key = Digit
            var countDigitInCell = new Dictionary<int, Dictionary<int, List<ICell>>>();
            foreach (var kv in digitInCell)
            {
                // Wenn ein Key/Digit nur in eine einzigen Zelle enthalten ist, muss dieses Digit gesetzt werden.
                if (kv.Value.Count == 1 && kv.Value.First().CandidateValue > 0)
                {
                    var cresult = kv.Value.First().SetDigit(kv.Key);
                    if (sudokuResult.Successful)
                    {
                        sudokuResult.ChildSudokuResult.Add(cresult);
                        cresult.ParentSudokuResult = sudokuResult;
                    }
                    if (!sudokuResult.Successful)
                    {
                        return;
                    }
                }
                else if (kv.Value.Count < 5)
                {
                    if (!countDigitInCell.ContainsKey(kv.Value.Count))
                    {
                        countDigitInCell.Add(kv.Value.Count, new Dictionary<int, List<ICell>>());
                    }
                    countDigitInCell[kv.Value.Count].Add(kv.Key, kv.Value);
                }
            }

            foreach (var kv in countDigitInCell)
            {
                var countCells = kv.Key; // Anzahl der Zellen in denen ein Digit enthalten ist
                if (kv.Value.Count < 2 || kv.Value.Count != countCells)
                {
                    continue;
                }

                var st = countCells switch
                {
                    2 => "Hidden2",
                    3 => "Hidden3",
                    4 => "Hidden4",
                    _ => "--No HiddenFiled possible ---",
                };
                var eq = true;
                List<ICell> last = null;
                foreach (var kv2 in kv.Value)
                {
                    if (last == null)
                    {
                        last = kv2.Value;
                        continue;
                    }
                    for (var i = 0; i < kv.Key; i++)
                    {
                        eq &= last[i].Equals(kv2.Value[i]);
                    }
                    last = kv2.Value;
                }

                if (eq)
                {
                    var cresult = sudokuResult.CreateChildResult();
                    cresult.EventInfoInResult = new SudokuEvent
                    {
                        ChangedCellBase = house,
                        Action = ECellAction.RemoveCandidate,
                        Value = 999999999,
                        SolveTechnik = st,
                    };
                    var found = false;
                    var cc = kv.Value.Values.First();
                    foreach (var c in cc)
                    {
                        for (var i = 1; i < Consts.DIMENSIONSQUARE + 1; i++)
                        {
                            if (kv.Value.ContainsKey(i))
                            {
                                continue;
                            }

                            found |= c.RemoveCandidate(i, cresult);
                        }
                    }
                    if (!found)
                    {
                        sudokuResult.ChildSudokuResult.Remove(cresult);
                    }
                }
            }
        }
    }
}
