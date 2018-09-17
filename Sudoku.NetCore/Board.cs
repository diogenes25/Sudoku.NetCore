﻿using DE.Onnen.Sudoku.SolveTechniques;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

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
        private const int ROW_CONTAINERTYPE = 0;
        private const int COL_CONTAINERTYPE = 1;
        private const int BLOCK_CONTAINERTYPE = 2;

        private List<SudokuHistoryItem> _history;
        private readonly House<Cell>[][] _container = new House<Cell>[Consts.DimensionSquare][];
        private bool _keepGoingWithChecks;
        private ISolveTechnique<Cell>[] _solveTechniques;

        public ReadOnlyCollection<SudokuHistoryItem> History { get { return this._history.AsReadOnly(); } }

        /// <summary>
        /// Loaded solvetechniques.
        /// </summary>
        public IList<ISolveTechnique<Cell>> SolveTechniques { get { return this._solveTechniques.Select(x => (ISolveTechnique<Cell>)x).ToList<ISolveTechnique<Cell>>(); } }

        public IHouse<Cell> GetHouse(HouseType houseType, int houseID)
        {
            return this._container[houseID][(int)houseType];
        }

        protected Cell[] _cells;

        public int Count
        {
            get { return this._cells.Count(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Cell this[int index] => this._cells[index];

        /// <summary>
        /// Changes in board
        /// </summary>
        public event System.EventHandler<SudokuEvent> BoardChangeEvent;

        /// <summary>
        /// Percent
        /// </summary>
        /// <returns>Percent</returns>
        public double SolvePercent
        {
            get
            {
                double currSolvePercent = 0;
                foreach (Cell c in this._cells)
                {
                    for (int i = 0; i < Consts.DimensionSquare; i++)
                    {
                        if (((1 << i) & c.CandidateValue) > 0)
                        {
                            currSolvePercent++;
                        }
                    }
                }
                return 100 - ((currSolvePercent / Consts.SolvePercentBase) * 100);
            }
        }

        /// <inheritdoc />
        public bool IsComplete()
        {
            int containerType = 0;
            for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
            {
                if (!this._container[containerIdx][containerType].IsComplete())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Some changes happend while solving. Another check is needed.
        /// </summary>
        internal void SomeChangesOccurs()
        {
            this._keepGoingWithChecks = true;
        }

        public Board()
        {
            Init();
        }

        public Board(string filePath)
        {
            LoadSolveTechnics(filePath);
            Init();
        }

        public Board(params ISolveTechnique<Cell>[] solveTechniques)
        {
            this._solveTechniques = solveTechniques;
            Init();
        }

        public Board(IEnumerable<int> uniqueCellIDs, params ISolveTechnique<Cell>[] solveTechniques)
        {
            this._solveTechniques = solveTechniques;
            Init();
            foreach (int uniqueCellID in uniqueCellIDs)
            {
                Cell c = Cell.CreateCellFromUniqueID(uniqueCellID);

                this._cells[c.ID]._candidateValueInternal = c.CandidateValue;
                this._cells[c.ID]._digit = c.Digit;
                this._cells[c.ID].IsGiven = c.Digit > 0;
            }
        }

        private void Init()
        {
            this._cells = new Cell[Consts.CountCell];

            for (int i = 0; i < Consts.CountCell; i++)
            {
                this._cells[i] = new Cell(i);
                this._cells[i].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }

            this._history = new List<SudokuHistoryItem>();

            Cell[][][] fieldcontainer;
            fieldcontainer = new Cell[3][][];
            fieldcontainer[ROW_CONTAINERTYPE] = new Cell[Consts.DimensionSquare][]; // Row
            fieldcontainer[COL_CONTAINERTYPE] = new Cell[Consts.DimensionSquare][]; // Col
            fieldcontainer[BLOCK_CONTAINERTYPE] = new Cell[Consts.DimensionSquare][]; // Block

            for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
            {
                fieldcontainer[ROW_CONTAINERTYPE][containerIdx] = new Cell[Consts.DimensionSquare];
                fieldcontainer[COL_CONTAINERTYPE][containerIdx] = new Cell[Consts.DimensionSquare];
                fieldcontainer[BLOCK_CONTAINERTYPE][containerIdx] = new Cell[Consts.DimensionSquare];

                for (int t = 0; t < Consts.DimensionSquare; t++)
                {
                    // Row 0,1,2,3,4,5,6,7,8
                    fieldcontainer[ROW_CONTAINERTYPE][containerIdx][t] = this._cells[t + (containerIdx * Consts.DimensionSquare)];
                    // Col 0,9,18,27,36
                    fieldcontainer[COL_CONTAINERTYPE][containerIdx][t] = this._cells[(t * Consts.DimensionSquare) + containerIdx];
                }

                // Block 0,1,2, 9,10,11, 18,19,20
                int blockCounter = 0;
                for (int zr = 0; zr < Consts.Dimension; zr++)
                {
                    for (int zc = 0; zc < Consts.Dimension; zc++)
                    {
                        int b = (containerIdx * Consts.Dimension) + (zc + (zr * Consts.DimensionSquare)) + ((containerIdx / Consts.Dimension) * Consts.DimensionSquare * 2);
                        fieldcontainer[BLOCK_CONTAINERTYPE][containerIdx][blockCounter++] = this._cells[b];
                    }
                }

                this._container[containerIdx] = new House<Cell>[3];
                for (int containerType = 0; containerType < 3; containerType++)
                {
                    this._container[containerIdx][containerType] = new House<Cell>(fieldcontainer[containerType][containerIdx], (HouseType)containerType, containerIdx);
                    foreach (Cell f in fieldcontainer[containerType][containerIdx])
                    {
                        f._fieldcontainters[containerType] = this._container[containerIdx][containerType];
                    }
                }
            }
        }

        private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.SomeChangesOccurs();
        }

        public SudokuLog SetDigit(int cellID, int digitToSet)
        {
            return this.SetDigit(cellID, digitToSet, false);
        }

        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="cellID">ID of cell</param>
        /// <param name="digitToSet">Set Digit to Cell</param>
        /// <param name="withSolve">true = Start solving with every solvetechnique (without backtrack) after digit was set.</param>
        public SudokuLog SetDigit(int cellID, int digitToSet, bool withSolve)
        {
            SudokuLog sudokuResult = new SudokuLog
            {
                EventInfoInResult = new SudokuEvent
                {
                    ChangedCellBase = null,
                    Action = CellAction.SetDigitInt,
                    SolveTechnik = "SetDigit",
                }
            };

            if (cellID < 0 || cellID > this._cells.Length)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = "Cell " + cellID + " is not in range";
                return sudokuResult;
            }
            sudokuResult.EventInfoInResult.ChangedCellBase = this._cells[cellID];

            this._cells[cellID].SetDigit(digitToSet, sudokuResult);
            if (sudokuResult.Successful)
            {
                this._cells[cellID].IsGiven = true;
                if (withSolve)
                {
                    this.Solve(sudokuResult);
                }

                this._history.Add(new SudokuHistoryItem(this, this._cells[cellID], sudokuResult));
            }
            else
            {
                SetHistory(this._history.Count - 1);
                this._cells[cellID].RemoveCandidate(digitToSet, sudokuResult);
            }
            return sudokuResult;
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

            for (int i = 0; i < Consts.CountCell; i++)
            {
                if (otherBoard[i].Digit > 0)
                {
                    this._cells[i].Digit = otherBoard[i].Digit;
                }
                else
                {
                    this._cells[i].CandidateValue = otherBoard[i].CandidateValue;
                }
            }
        }

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
        public int[] CreateSimpleBoard()
        {
            return this.Select(x => x.GetUniqueID()).ToArray();
        }

        public void SetHistory(int historyId)
        {
            if (historyId < 0 || historyId >= this._history.Count)
            {
                return;
            }

            for (int i = 0; i < this._history[historyId].BoardInt.Count; i++)
            {
                if (this._history[historyId].BoardInt[i] < 0)
                {
                    try
                    {
                        this._cells[i].Digit = this._history[historyId].BoardInt[i] * -1;
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    this._cells[i].CandidateValue = this._history[historyId].BoardInt[i];
                }
            }
        }

        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">Log</param>
        public bool Solve(SudokuLog sudokuResult)
        {
            SudokuLog tmpSudokuResult = sudokuResult;
            if (tmpSudokuResult == null)
            {
                tmpSudokuResult = new SudokuLog();
            }

            do
            {
                this._keepGoingWithChecks = false;
                for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
                {
                    for (int containerType = 0; containerType < 3; containerType++)
                    {
                        if (this._container[containerIdx][containerType].IsComplete())
                        {
                            continue;
                        }

                        if (this._solveTechniques != null && this._solveTechniques.Length > 0)
                        {
                            foreach (ISolveTechnique<Cell> st in this._solveTechniques)
                            {
                                if (st.IsActive)
                                {
                                    if (!this._container[containerIdx][containerType].ReCheck && st.CellView == ECellView.OnlyHouse)
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        st.SolveHouse(this, this._container[containerIdx][containerType], tmpSudokuResult);
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
                        this._container[containerIdx][containerType].ReCheck = false;
                    }
                }
            } while (this._keepGoingWithChecks);
            return tmpSudokuResult.Successful;
        }

        public SudokuLog Backtracking()
        {
            SudokuLog tmpSudokuResult = new SudokuLog();

            if (!BacktrackingContinue((Board)this.Clone()))
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
            for (int i = 0; i < this.Count; i++)
            {
                this._cells[i].Digit = 0;
            }

            this._history.Clear();

            for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
            {
                for (int containerType = 0; containerType < 3; containerType++)
                {
                    this._container[containerIdx][containerType].Clear();
                }
            }
        }

        private bool BacktrackingContinue(Board board)
        {
            bool isComplete = false;
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

            for (int i = 0; i < this._cells.Length; i++)
            {
                if (board[i].Digit == 0)
                {
                    ReadOnlyCollection<int> posDigit = board[i].Candidates;
                    foreach (int x in posDigit)
                    {
                        Board newBoard = (Board)board.Clone();
                        SudokuLog result = newBoard.SetDigit(i, x, true);
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
                            for (int s = 0; s < this._cells.Length; s++)
                            {
                                this._cells[s]._candidateValueInternal = 0;
                                this._cells[s]._digit = newBoard._cells[s].Digit;
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

        #region ICloneable Members

        /// <summary>
        /// Clone Board.
        /// </summary>
        /// <returns>copy of board</returns>
        public object Clone()
        {
            return new Board(this.CreateSimpleBoard(), this._solveTechniques);
        }

        #endregion ICloneable Members

        private void LoadSolveTechnics(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            string[] files = Directory.GetFiles(filePath, "*.dll");
            if (files.Count() < 1)
            {
                return;
            }

            this._solveTechniques = new ASolveTechnique<Cell>[files.Count()];
            int fileCount = 0;
            foreach (string file in files)
            {
                ISolveTechnique<Cell> st = SudokuSolveTechniqueLoader<Cell>.LoadSolveTechnic(file);
                this._solveTechniques[fileCount++] = st;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ICell cell in this)
            {
                sb.Append(cell.Digit);
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;
            if (obj != null && obj is IBoard<Cell>)
            {
                IBoard<Cell> nb = (IBoard<Cell>)obj;
                return Equals(nb);
            }
            return retVal;
        }

        public override int GetHashCode()
        {
            int retHash = 0;
            foreach (Cell c in this)
            {
                retHash += (c.Digit + (1 << (c.ID % 9)));
            }
            return retHash;
        }

        public bool Equals(IBoard<Cell> other)
        {
            bool retVal = true;
            if (other == null)
            {
                return false;
            }

            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                retVal &= this[i].Digit == other[i].Digit;
            }
            return retVal;
        }

        public bool Equals(Board other)
        {
            return Equals((IBoard<Cell>)other);
        }

        #region IEnumerable<ICell> Members

        public IEnumerator<Cell> GetEnumerator()
        {
            return this._cells.Select(x => (Cell)x).GetEnumerator();
        }

        #endregion IEnumerable<ICell> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._cells.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}