﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using DE.Onnen.Sudoku.SolveTechniques;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// https://github.com/diogenes25/Sudoku
    /// http://diogenes25.github.io/Sudoku/
    /// http://sudoku-solutions.com/
    /// http://hodoku.sourceforge.net/de/index.php
    /// http://forum.enjoysudoku.com/
    /// http://www.sadmansoftware.com/sudoku/solvingtechniques.htm
    /// http://www.setbb.com/phpbb/viewtopic.php?t=379&mforum=sudoku
    /// http://www.playr.co.uk/sudoku/dictionary.php
    /// http://www.sudocue.net/glossary.php
    /// http://walter.bislins.ch/projekte/index.asp?page=Sudoku
    /// </summary>
    public class Board : ICloneable, IBoard<Cell>, IEquatable<Board>
    {
        protected Cell[] _cells;
        private const int BLOCK_CONTAINERTYPE = 2;
        private const int COL_CONTAINERTYPE = 1;
        private const int ROW_CONTAINERTYPE = 0;
        private readonly House<Cell>[][] _container = new House<Cell>[Consts.DIMENSIONSQUARE][];
        private List<SudokuHistoryItem> _history;
        private bool _keepGoingWithChecks;
        private ISolveTechnique<Cell>[] _solveTechniques;

        public Board() => Init();

        public Board(string filePath)
        {
            LoadSolveTechnics(filePath);
            Init();
        }

        public Board(params ISolveTechnique<Cell>[] solveTechniques)
        {
            _solveTechniques = solveTechniques;
            Init();
        }

        public Board(IEnumerable<int> uniqueCellIDs, params ISolveTechnique<Cell>[] solveTechniques)
        {
            _solveTechniques = solveTechniques;
            Init();
            foreach (var uniqueCellID in uniqueCellIDs)
            {
                var c = Cell.CreateCellFromUniqueID(uniqueCellID);

                _cells[c.ID]._candidateValueInternal = c.CandidateValue;
                _cells[c.ID]._digit = c.Digit;
                _cells[c.ID].IsGiven = c.Digit > 0;
            }
        }

        /// <summary>
        /// Changes in board
        /// </summary>
        public event System.EventHandler<SudokuEvent> BoardChangeEvent;

        public int Count => _cells.Length;

        public ReadOnlyCollection<SudokuHistoryItem> History => _history.AsReadOnly();

        /// <summary>
        /// Percent
        /// </summary>
        /// <returns>Percent</returns>
        public double SolvePercent
        {
            get
            {
                double currSolvePercent = 0;
                foreach (var c in _cells)
                {
                    for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
                    {
                        if (((1 << i) & c.CandidateValue) > 0)
                        {
                            currSolvePercent++;
                        }
                    }
                }
                return 100 - ((currSolvePercent / Consts.SOLVEPERCENTBASE) * 100);
            }
        }

        /// <summary>
        /// Loaded solvetechniques.
        /// </summary>
        public IList<ISolveTechnique<Cell>> SolveTechniques => _solveTechniques.Select(x => (ISolveTechnique<Cell>)x).ToList<ISolveTechnique<Cell>>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Cell this[int index] => _cells[index];

        public SudokuLog Backtracking()
        {
            var tmpSudokuResult = new SudokuLog();

            if (!BacktrackingContinue((Board)Clone()))
            {
                tmpSudokuResult.Successful = false;
                tmpSudokuResult.ErrorMessage = "Sudoku hat keine Lösung";
            }

            return tmpSudokuResult;
        }

        /// <summary>
        /// Reset Board.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < Count; i++)
            {
                _cells[i].Digit = 0;
            }

            _history.Clear();

            for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
            {
                for (var containerType = 0; containerType < 3; containerType++)
                {
                    _container[containerIdx][containerType].Clear();
                }
            }
        }

        /// <summary>
        /// Clone Board.
        /// </summary>
        /// <returns>copy of board</returns>
        public object Clone() => new Board(CreateSimpleBoard(), _solveTechniques);

        // Convert a Board to an int-Array
        /// <summary>
        /// Convert a Board to an int-Array
        /// </summary>
        /// <remarks>
        /// Positiv value = Candidates as bitmask.<br/>
        /// Negativ value = Digit.
        /// </remarks>
        /// <param name="board"></param>
        /// <returns></returns>
        public int[] CreateSimpleBoard() => this.Select(x => x.GetUniqueID()).ToArray();

        public override bool Equals(object obj)
        {
            if (obj is not null and IBoard<Cell> board)
            {
                var nb = board;
                return Equals(nb);
            }
            return false;
        }

        public bool Equals(IBoard<Cell> other)
        {
            var retVal = true;
            if (other == null)
            {
                return false;
            }

            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                retVal &= this[i].Digit == other[i].Digit;
            }
            return retVal;
        }

        public bool Equals(Board other) => Equals((IBoard<Cell>)other);

        public IEnumerator<Cell> GetEnumerator() => _cells.Select(x => (Cell)x).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cells.GetEnumerator();

        public override int GetHashCode()
        {
            var retHash = 0;
            foreach (var c in this)
            {
                retHash += (c.Digit + (1 << (c.ID % 9)));
            }
            return retHash;
        }

        public IHouse<Cell> GetHouse(HouseType houseType, int houseID) => _container[houseID][(int)houseType];

        /// <inheritdoc />
        public bool IsComplete()
        {
            var containerType = 0;
            for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
            {
                if (!_container[containerIdx][containerType].IsComplete())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Set Cells from otherBoard to this Board.
        /// </summary>
        /// <param name="otherBoard">Another Board</param>
        public void SetBoard(IBoard<Cell> otherBoard)
        {
            if (otherBoard == null)
            {
                return;
            }

            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                if (otherBoard[i].Digit > 0)
                {
                    _cells[i].Digit = otherBoard[i].Digit;
                }
                else
                {
                    _cells[i].CandidateValue = otherBoard[i].CandidateValue;
                }
            }
        }

        public SudokuLog SetDigit(int cellID, int digitToSet) => SetDigit(cellID, digitToSet, false);

        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="cellID">ID of cell</param>
        /// <param name="digitToSet">Set Digit to Cell</param>
        /// <param name="withSolve">true = Start solving with every solvetechnique (without backtrack) after digit was set.</param>
        public SudokuLog SetDigit(int cellID, int digitToSet, bool withSolve)
        {
            var sudokuResult = new SudokuLog
            {
                EventInfoInResult = new SudokuEvent
                {
                    ChangedCellBase = null,
                    Action = CellAction.SetDigitInt,
                    SolveTechnik = "SetDigit",
                }
            };

            if (cellID < 0 || cellID > _cells.Length)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = "Cell " + cellID + " is not in range";
                return sudokuResult;
            }
            sudokuResult.EventInfoInResult.ChangedCellBase = _cells[cellID];

            _cells[cellID].SetDigit(digitToSet, sudokuResult);
            if (sudokuResult.Successful)
            {
                _cells[cellID].IsGiven = true;
                if (withSolve)
                {
                    Solve(sudokuResult);
                }

                _history.Add(new SudokuHistoryItem(this, _cells[cellID], sudokuResult));
            }
            else
            {
                SetHistory(_history.Count - 1);
                _cells[cellID].RemoveCandidate(digitToSet, sudokuResult);
            }
            return sudokuResult;
        }

        public void SetHistory(int historyId)
        {
            if (historyId < 0 || historyId >= _history.Count)
            {
                return;
            }

            for (var i = 0; i < _history[historyId].BoardInt.Count; i++)
            {
                if (_history[historyId].BoardInt[i] < 0)
                {
                    try
                    {
                        _cells[i].Digit = _history[historyId].BoardInt[i] * -1;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    _cells[i].CandidateValue = _history[historyId].BoardInt[i];
                }
            }
        }

        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">Log</param>
        public bool Solve(SudokuLog sudokuResult)
        {
            var tmpSudokuResult = sudokuResult;
            if (tmpSudokuResult == null)
            {
                tmpSudokuResult = new SudokuLog();
            }

            do
            {
                _keepGoingWithChecks = false;
                for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
                {
                    for (var containerType = 0; containerType < 3; containerType++)
                    {
                        if (_container[containerIdx][containerType].IsComplete())
                        {
                            continue;
                        }

                        if (_solveTechniques != null && _solveTechniques.Length > 0)
                        {
                            foreach (var st in _solveTechniques)
                            {
                                if (st.IsActive)
                                {
                                    if (!_container[containerIdx][containerType].ReCheck && st.CellView == ECellView.OnlyHouse)
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        st.SolveHouse(this, _container[containerIdx][containerType], tmpSudokuResult);
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                    if (!tmpSudokuResult.Successful)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        _container[containerIdx][containerType].ReCheck = false;
                    }
                }
            } while (_keepGoingWithChecks);
            return tmpSudokuResult.Successful;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (ICell cell in this)
            {
                sb.Append(cell.Digit);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Some changes happend while solving. Another check is needed.
        /// </summary>
        internal void SomeChangesOccurs() => _keepGoingWithChecks = true;

        private bool BacktrackingContinue(Board board)
        {
            bool isComplete;
            try
            {
                isComplete = board.IsComplete();
            }
            catch (Exception)
            {
                return false;
            }

            if (isComplete)
            {
                return true;
            }

            for (var i = 0; i < _cells.Length; i++)
            {
                if (board[i].Digit == 0)
                {
                    var posDigit = board[i].Candidates;
                    foreach (var x in posDigit)
                    {
                        var newBoard = (Board)board.Clone();
                        var result = newBoard.SetDigit(i, x, true);
                        newBoard[i].IsGiven = false;
                        BoardChangeEvent?.Invoke(newBoard, new SudokuEvent { Action = CellAction.SetDigitInt, ChangedCellBase = newBoard[i] });
                        if (!result.Successful)
                        {
                            continue;
                        }

                        try
                        {
                            isComplete = newBoard.IsComplete();
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        if (isComplete)
                        {
                            for (var s = 0; s < _cells.Length; s++)
                            {
                                _cells[s]._candidateValueInternal = 0;
                                _cells[s]._digit = newBoard._cells[s].Digit;
                            }

                            return true;
                        }

                        if (BacktrackingContinue(newBoard))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => SomeChangesOccurs();

        private void Init()
        {
            _cells = new Cell[Consts.COUNTCELL];

            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                _cells[i] = new Cell(i);
                _cells[i].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }

            _history = new List<SudokuHistoryItem>();

            Cell[][][] fieldcontainer;
            fieldcontainer = new Cell[3][][];
            fieldcontainer[ROW_CONTAINERTYPE] = new Cell[Consts.DIMENSIONSQUARE][]; // Row
            fieldcontainer[COL_CONTAINERTYPE] = new Cell[Consts.DIMENSIONSQUARE][]; // Col
            fieldcontainer[BLOCK_CONTAINERTYPE] = new Cell[Consts.DIMENSIONSQUARE][]; // Block

            for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
            {
                fieldcontainer[ROW_CONTAINERTYPE][containerIdx] = new Cell[Consts.DIMENSIONSQUARE];
                fieldcontainer[COL_CONTAINERTYPE][containerIdx] = new Cell[Consts.DIMENSIONSQUARE];
                fieldcontainer[BLOCK_CONTAINERTYPE][containerIdx] = new Cell[Consts.DIMENSIONSQUARE];

                for (var t = 0; t < Consts.DIMENSIONSQUARE; t++)
                {
                    // Row 0,1,2,3,4,5,6,7,8
                    fieldcontainer[ROW_CONTAINERTYPE][containerIdx][t] = _cells[t + (containerIdx * Consts.DIMENSIONSQUARE)];
                    // Col 0,9,18,27,36
                    fieldcontainer[COL_CONTAINERTYPE][containerIdx][t] = _cells[(t * Consts.DIMENSIONSQUARE) + containerIdx];
                }

                // Block 0,1,2, 9,10,11, 18,19,20
                var blockCounter = 0;
                for (var zr = 0; zr < Consts.DIMENSION; zr++)
                {
                    for (var zc = 0; zc < Consts.DIMENSION; zc++)
                    {
                        var b = (containerIdx * Consts.DIMENSION) + (zc + (zr * Consts.DIMENSIONSQUARE)) + ((containerIdx / Consts.DIMENSION) * Consts.DIMENSIONSQUARE * 2);
                        fieldcontainer[BLOCK_CONTAINERTYPE][containerIdx][blockCounter++] = _cells[b];
                    }
                }

                _container[containerIdx] = new House<Cell>[3];
                for (var containerType = 0; containerType < 3; containerType++)
                {
                    _container[containerIdx][containerType] = new House<Cell>(fieldcontainer[containerType][containerIdx], (HouseType)containerType, containerIdx);
                    foreach (var f in fieldcontainer[containerType][containerIdx])
                    {
                        f._fieldcontainters[containerType] = _container[containerIdx][containerType];
                    }
                }
            }
        }

        private void LoadSolveTechnics(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var files = Directory.GetFiles(filePath, "*.dll");
            if (files.Length < 1)
            {
                return;
            }

            _solveTechniques = new ASolveTechnique<Cell>[files.Length];
            var fileCount = 0;
            foreach (var file in files)
            {
                var st = SudokuSolveTechniqueLoader<Cell>.LoadSolveTechnic(file);
                _solveTechniques[fileCount++] = st;
            }
        }
    }
}
