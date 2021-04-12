namespace DE.Onnen.Sudoku.SolveTechniques
{
    public class LockedCandidates<C> : ASolveTechnique<C> where C : ICell
    {
        public LockedCandidates() => Info = SolveTechniqueInfo.GetTechniqueInfo(caption: "LockedCandidates", descr: "LockedCandidates");

        public override ECellView CellView => ECellView.GlobalView;

        public override void SolveHouse(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult)
        {
            DigitInBlock(board, house, sudokuResult);
            if (house.HType == HouseType.Box)
            {
                DigitInBlock(board, house, sudokuResult, true);
            }
        }

        /// <summary>
        /// Sucht Nummern die sich nur in einem Drittel der Container-Cells enthalten sind.
        /// </summary>
        /// <remarks>
        /// Die Nummern sind als |-Verknüpfung in dem Int-Array enthalten.<br/>
        /// Das Array enthält drei Elemente. Je nachdem ob die Digit im ersten (0) zweiten (1) oder drittem Drittel (2) enthalten sind.
        /// </remarks>
        /// <returns>Drei Int-Werte [erster, zweiter oder dritter Part] der gefundenen Nummern |-Verknüpft.</returns>
        private static int[] CheckInBlockpartOfRowCol(IHouse<C> house, bool verticalBlock = false)
        {
            var valueInRow = new int[Consts.DIMENSION];
            var valueOnlyInOneRow = new int[3];

            // Fügt alle noch möglichen Nummer pro Part zusammen.
            for (var p = 0; p < Consts.DIMENSIONSQUARE; p++)
            {
                if (verticalBlock)
                {
                    valueInRow[(p % Consts.DIMENSION)] |= house[p].CandidateValue;
                }
                else
                {
                    valueInRow[p / Consts.DIMENSION] |= house[p].CandidateValue;
                }
            }

            // Finde die Nummern die nur in einem Part enthalten sind
            for (var part = 0; part < 3; part++)
            {
                var addVal = 0; // BaseValue der anderen beiden Parts.
                                // Addiere die Nummern der beiden anderen Parts.
                for (var p = 0; p < 3; p++)
                {
                    if (part == p)
                    {
                        continue;
                    }

                    addVal |= valueInRow[p];
                }
                valueOnlyInOneRow[part] = valueInRow[part] ^ addVal; // XOR-Verknüpfung um nur die unterschiedlichen Nummern zu erhalten.
                valueOnlyInOneRow[part] &= valueInRow[part]; // UND-Verknüpfung um nur die Werte zu erhalten auch zuvor in dem PArt waren.
            }
            return valueOnlyInOneRow;
        }

        private static void DigitInBlock(IBoard<C> board, IHouse<C> house, SudokuLog sudokuResult, bool verticalBlock = false)
        {
            var valueOnlyInOnePart = CheckInBlockpartOfRowCol(house, verticalBlock);
            for (var x = 0; x < 3; x++)
            {
                if (valueOnlyInOnePart[x] > 0)
                {
                    _ = house.HType;
                    var st = "LockedCandidatesClaiming";
                    int houseIdx;
                    HouseType cellInContainertype;
                    switch (house.HType)
                    {
                        case HouseType.Row:
                            houseIdx = ((int)(house.ID / Consts.DIMENSION)) * 3 + x;
                            cellInContainertype = HouseType.Box;
                            break;

                        case HouseType.Col:
                            houseIdx = x * 3 + (house.ID / Consts.DIMENSION);
                            cellInContainertype = HouseType.Box;
                            break;

                        case HouseType.Box:
                            st = "LockedCandidatesPointing";
                            houseIdx = (verticalBlock) ? (house.ID % 3) * 3 + x : (house.ID / 3) * 3 + x;
                            cellInContainertype = (verticalBlock) ? HouseType.Col : HouseType.Row;
                            break;

                        default:
                            throw new System.ArgumentException($"HType {house.HType} ist unknown");
                    }

                    var pos = -1;
                    foreach (ICell c in board.GetHouse(cellInContainertype, houseIdx)) // Cells des Row
                    {
                        pos++;
                        if (
                            ((house.HType == HouseType.Row) && (house.ID % Consts.DIMENSION) == (pos / Consts.DIMENSION))
                            || ((house.HType == HouseType.Col) && (house.ID % Consts.DIMENSION) == (pos % Consts.DIMENSION))
                            || ((house.HType == HouseType.Box)
                                && ((!verticalBlock && (house.ID % Consts.DIMENSION) == (pos / Consts.DIMENSION))
                                    || (verticalBlock && (house.ID / Consts.DIMENSION) == (pos / Consts.DIMENSION))
                                   )
                               )
                            )
                        {
                            continue;
                        }

                        if (!RemoveMultiValue(c, valueOnlyInOnePart[x], sudokuResult, house, st))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static bool RemoveMultiValue(ICell c, int removeValue, SudokuLog sudokuResult, IHasCandidates resultContainer, string solveTechnik)
        {
            for (var dc = 0; dc < Consts.DIMENSIONSQUARE; dc++)
            {
                if ((removeValue & (1 << dc)) > 0)
                {
                    if ((c.CandidateValue & (1 << dc)) > 0)
                    {
                        var child = sudokuResult.CreateChildResult();
                        if (c.RemoveCandidate(dc + 1, child))
                        {
                            child.EventInfoInResult = new SudokuEvent
                            {
                                ChangedCellBase = resultContainer,
                                Action = CellAction.RemoveCandidate,
                                SolveTechnik = solveTechnik,
                                Value = dc + 1,
                            };
                        }
                        else
                        {
                            sudokuResult.ChildSudokuResult.Remove(child);
                        }
                        if (!sudokuResult.Successful)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
