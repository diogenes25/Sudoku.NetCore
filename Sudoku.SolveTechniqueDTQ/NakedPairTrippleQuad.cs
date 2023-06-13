using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku.SolveTechniques
{
    /// <summary>
    /// Naked Pair, Triplet, Quad (aka Locked Set, Naked Subset, Disjoint Subset)
    /// <remarks>
    /// If two cells in the same house (row, column or block) have only the same two candidates,
    /// then those candidates can be removed from other cells in that house (row, column or block).
    /// This technique is known as "naked pair" if two candidates are involved, "naked triplet" if three, or "naked quad" if four.
    /// </remarks>
    /// </summary>
    public class NakedPairTrippleQuad<C> : ASolveTechnique<C> where C : ICell
    {
        public NakedPairTrippleQuad() => Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "Naked PairTripleQuad", descr: "Naked Pair Triple and/or Quad.");

        public override ECellView CellView => ECellView.OnlyHouse;

        public override void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult)
        {
            // Key = BaseValue, Anz Possible
            var nakedMore = new Dictionary<int, List<C>>();
            foreach (var c in house)
            {
                var val = c.CandidateValue;
                if (!nakedMore.ContainsKey(val))
                {
                    nakedMore.Add(val, new List<C>());
                }

                nakedMore[val].Add(c);
            }

            foreach (var kv in nakedMore)
            {
                if (kv.Value.Count > 5)
                {
                    return;
                }

                var count = kv.Value.First().Candidates.Count;
                if (kv.Value.Count == count)
                {
                    var st = "Naked" + count;
                    var cresult = sudokuResult.CreateChildResult();
                    cresult.EventInfoInResult = new SudokuEvent
                    {
                        Value = 0,
                        ChangedCellBase = house,
                        Action = CellAction.RemoveCandidate,
                        SolveTechnik = st,
                    };
                    var found = false;
                    foreach (var c in house)
                    {
                        if (kv.Value.Contains(c))
                        {
                            continue;
                        }
                        ICell kvc = kv.Value.First();
                        foreach (var d in kvc.Candidates)
                        {
                            found |= c.RemoveCandidate(d, cresult);
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
