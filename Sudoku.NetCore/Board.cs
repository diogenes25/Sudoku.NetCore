//-----------------------------------------------------------------------
// <copyright file="Board.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Represents a Sudoku board.
    /// </summary>
    public sealed partial class Board : ICloneable, IBoard<Cell>, IEquatable<Board>
    {
        /// <summary>
        /// The cells of the Sudoku board.
        /// </summary>
        private Cell[] _cells = new Cell[Consts.COUNTCELL];

        private const int BLOCK_CONTAINERTYPE = 2;
        private const int COL_CONTAINERTYPE = 1;
        private const int ROW_CONTAINERTYPE = 0;
        private readonly House[][] _container = new House[Consts.DIMENSIONSQUARE][];
        private readonly List<SudokuHistoryItem> _history = [];
        private bool _keepGoingWithChecks;
        private List<ISolveTechnique<Cell>>? _solveTechniques;
        private readonly ILogger<Board> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class.
        /// The default constructor will create a new board with all cells set to 0.
        /// </summary>
        public Board()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            _logger = loggerFactory.CreateLogger<Board>();
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with the specified solve techniques and logger.
        /// </summary>
        /// <param name="solveTechniques">The solve techniques to use.</param>
        /// <param name="logger">The logger to use.</param>
        public Board(IEnumerable<ISolveTechnique<Cell>>? solveTechniques, ILogger<Board> logger)
        {
            _solveTechniques = solveTechniques?.ToList();
            _logger = logger ?? NullLogger<Board>.Instance;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with the solve techniques loaded from the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file containing the solve techniques.</param>
        public Board(string filePath)
        {
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            }).CreateLogger<Board>();

            LoadSolveTechnics(filePath);
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with the specified unique cell IDs, solve techniques, and logger.
        /// </summary>
        /// <param name="uniqueCellIDs">The unique cell IDs to fill the board with.</param>
        /// <param name="solveTechniques">The solve techniques to use.</param>
        /// <param name="logger">The logger to use.</param>
        public Board([NotNull] IEnumerable<int> uniqueCellIDs, IEnumerable<ISolveTechnique<Cell>>? solveTechniques = null, ILogger<Board>? logger = null)
        {
            _solveTechniques = solveTechniques?.ToList();
            _logger = logger ?? NullLogger<Board>.Instance;
            Init();
            FillBoardWithUniqueCellIDs(uniqueCellIDs);
        }

        /// <summary>
        /// Occurs when changes are made to the board.
        /// </summary>
        public event System.EventHandler<SudokuEvent>? BoardChangeEvent;

        /// <summary>
        /// Gets the percentage of the board that has been solved.
        /// The percentage is calculated on the basis of the possible candidates.
        /// So the basis of the calculation is 9 * 9 fields with 9 candidates each(729).
        /// </summary>
        public double SolvePercent
        {
            get
            {
                var currSolvePercent = 0;
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
        /// Gets or sets the cell at the specified index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The cell at the specified index.</returns>
        public Cell this[int index] => _cells[index];

        /// <summary>
        /// Performs backtracking to solve the Sudoku board.
        /// </summary>
        /// <returns>The result of the backtracking operation.</returns>
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
        /// Resets the board by setting all cells to 0.
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
        /// Determines whether the specified object is equal to the current board.
        /// </summary>
        /// <param name="obj">The object to compare with the current board.</param>
        /// <returns>true if the specified object is equal to the current board; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is not null and IBoard<Cell> board && Equals(board);

        /// <summary>
        /// Creates a copy of the board.
        /// </summary>
        /// <returns>A copy of the board.</returns>
        public object Clone() => new Board(CreateSimpleBoard(), _solveTechniques);

        /// <summary>
        /// Determines whether the specified board is equal to the current board.
        /// </summary>
        /// <param name="other">The board to compare with the current board.</param>
        /// <returns>true if the specified board is equal to the current board; otherwise, false.</returns>
        public bool Equals(IBoard<Cell>? other)
        {
            if (other == null)
            {
                return false;
            }

            var retVal = true;

            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                retVal &= this[i].Digit == other[i].Digit;
            }
            return retVal;
        }

        /// <summary>
        /// Determines whether the specified board is equal to the current board.
        /// </summary>
        /// <param name="other">The board to compare with the current board.</param>
        /// <returns>true if the specified board is equal to the current board; otherwise, false.</returns>
        public bool Equals(Board? other) => Equals(other: other as IBoard<Cell>);

        /// <summary>
        /// Fills the board with the specified unique cell IDs.
        /// </summary>
        /// <param name="uniqueCellIDs">The unique cell IDs to fill the board with.</param>
        public void FillBoardWithUniqueCellIDs([NotNull] IEnumerable<int> uniqueCellIDs)
        {
            foreach (var uniqueCellID in uniqueCellIDs)
            {
                var c = Cell.CreateCellFromUniqueID(uniqueCellID);

                _cells[c.ID]._candidateValueInternal = c.CandidateValue;
                _cells[c.ID]._digit = c.Digit;
                _cells[c.ID].IsGiven = c.Digit > 0;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells of the board.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the cells of the board.</returns>
        public IEnumerator<Cell> GetEnumerator() => _cells.Select(x => (Cell)x).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the cells of the board.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the cells of the board.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cells.GetEnumerator();

        /// <summary>
        /// Returns the hash code for the board.
        /// </summary>
        /// <returns>The hash code for the board.</returns>
        public override int GetHashCode()
        {
            var retHash = 0;
            foreach (var c in this)
            {
                retHash += (c.Digit + (1 << (c.ID % 9)));
            }
            return retHash;
        }

        /// <summary>
        /// Gets the house of the specified type and ID.
        /// </summary>
        /// <param name="houseType">The type of the house.</param>
        /// <param name="houseID">The ID of the house.</param>
        /// <returns>The house of the specified type and ID.</returns>
        public IHouse<Cell> GetHouse(EHouseType houseType, int houseID) => _container[houseID][(int)houseType];

        /// <summary>
        /// Determines whether the board is complete (all cells are filled).
        /// </summary>
        /// <returns>true if the board is complete; otherwise, false.</returns>
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
        /// Sets the digit at the specified cell ID.
        /// </summary>
        /// <param name="cellID">The ID of the cell.</param>
        /// <param name="digitToSet">The digit to set.</param>
        /// <returns>The result of setting the digit.</returns>
        public SudokuLog SetDigit(int cellID, int digitToSet) => SetDigit(cellID, digitToSet, false);

        /// <summary>
        /// Sets the digit at the specified cell ID.
        /// </summary>
        /// <param name="cellID">The ID of the cell.</param>
        /// <param name="digitToSet">The digit to set.</param>
        /// <param name="withSolve">true to start solving with every solve technique (without backtracking) after the digit is set; otherwise, false.</param>
        /// <returns>The result of setting the digit.</returns>
        public SudokuLog SetDigit(int cellID, int digitToSet, bool withSolve)
        {
            var sudokuResult = new SudokuLog
            {
                EventInfoInResult = new SudokuEvent
                {
                    ChangedCellBase = null,
                    Action = ECellAction.SetDigitInt,
                    SolveTechnique = "SetDigit",
                }
            };

            if (cellID < 0 || cellID > _cells.Length)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = $"Cell {cellID} is not in range";
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
                if (_history.Count > 0)
                {
                    SetHistory(_history.Count - 1);
                }
                _cells[cellID].RemoveCandidate(digitToSet, sudokuResult);
            }
            return sudokuResult;
        }

        /// <summary>
        /// Starts solving the Sudoku board using the loaded solve techniques.
        /// </summary>
        /// <returns>The result of the solving process.</returns>
        public SudokuLog StartSolve()
        {
            var initSudokuLog = new SudokuLog();
            Solve(initSudokuLog);
            return initSudokuLog;
        }

        /// <summary>
        /// Count of cells (normaly 81 = 9*9)
        /// </summary>
        public int Count => _cells.Length;

        /// <summary>
        /// History of Steps during the solvingprocess
        /// </summary>
        public ReadOnlyCollection<SudokuHistoryItem> History => _history.AsReadOnly();

        /// <summary>
        /// Loaded solvetechniques.
        /// </summary>
        public IList<ISolveTechnique<Cell>>? SolveTechniques => _solveTechniques?.Select(x => (ISolveTechnique<Cell>)x).ToList<ISolveTechnique<Cell>>();

        /// <summary>
        /// Convert a Board to an int-Array
        /// </summary>
        /// <remarks>
        /// Positiv value = Candidates as bitmask.<br/>
        /// Negativ value = Digit.
        /// </remarks>
        /// <returns>Int-Array that represents the board</returns>
        public int[] CreateSimpleBoard() => this.Select(x => x.GetUniqueID()).ToArray();

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

        private void SetHistory(int historyId)
        {
            if (historyId < 0 || historyId >= _history.Count)
            {
                LogError_DigitCouldNotSet(_logger, historyId, (_history.Count));
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
                    catch (Exception ex)
                    {
                        LogError_DigitCouldNotSet(_logger, i, historyId, ex);
                        //_logger?.LogError(ex, LoggerMessage. "Error in SetHistory: _cells[{CellId}].Digit = _history[HistoryId}].BoardInt[{CellId}]", i, historyId);
                    }
                }
                else
                {
                    _cells[i].CandidateValue = _history[historyId].BoardInt[i];
                }
            }
        }

        [LoggerMessage(EventId = 100, Level = LogLevel.Warning, Message = "SetHistory with parameter historyId (value:{historyId}) is out of range (0 - {hisCount})")]
        private static partial void LogError_DigitCouldNotSet(ILogger<Board> logger, int historyId, int hisCount);

        [LoggerMessage(EventId = 110, Level = LogLevel.Error, Message = "Error in SetHistory: _cells[{cellId}].Digit = _history[{historyId}].BoardInt[{cellId}]")]
        private static partial void LogError_DigitCouldNotSet(ILogger<Board> logger, int cellId, int historyId, Exception ex);

        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">Log</param>
        private void Solve(SudokuLog? sudokuResult)
        {
            var tmpSudokuResult = sudokuResult ?? new SudokuLog();
            //tmpSudokuResult ??= new SudokuLog();

            if (_solveTechniques is null || _solveTechniques.Count < 1)
            {
                if (sudokuResult is not null)
                {
                    sudokuResult.Successful = true;
                    sudokuResult.ErrorMessage = "No SolveTechnique is set";
                }
                return;
            }

            do
            {
                _keepGoingWithChecks = false;

                foreach (var st in _solveTechniques.Where(t => t.IsActive))
                {
                    try
                    {
                        st.SolveBoard(this, tmpSudokuResult);
                    }
                    catch (Exception ex)
                    {
                        var log = tmpSudokuResult.CreateChildResult();
                        log.ErrorMessage = ex.Message;
                        log.Successful = false;

                        return;
                    }
                    if (!tmpSudokuResult.Successful)
                    {
                        return;
                    }
                }

                for (var containerIdx = 0; containerIdx < Consts.DIMENSIONSQUARE; containerIdx++)
                {
                    for (var containerType = 0; containerType < 3; containerType++)
                    {
                        if (_container[containerIdx][containerType].IsComplete())
                        {
                            continue;
                        }

                        foreach (var st in _solveTechniques.Where(t => t.IsActive))
                        {
                            if (!_container[containerIdx][containerType].ReCheck || _container[containerIdx][containerType].CandidateValue == 0) //&& st.CellView == ECellView.OnlyHouse)
                            {
                                continue;
                            }

                            try
                            {
                                st.SolveHouse(this, _container[containerIdx][containerType], tmpSudokuResult);
                            }
                            catch (Exception ex)
                            {
                                var log = tmpSudokuResult.CreateChildResult();
                                log.ErrorMessage = ex.Message;
                                log.Successful = false;

                                return;
                            }
                            if (!tmpSudokuResult.Successful)
                            {
                                return;
                            }
                        }
                        _container[containerIdx][containerType].ReCheck = false;
                    }
                }
            } while (_keepGoingWithChecks);
        }

        /// <inheritdoc/>
        public override string ToString() => string.Join("", this.Select(x => x.Digit));

        /// <summary>
        /// Some changes happend while solving. Another check is needed.
        /// </summary>
        internal void SomeChangesOccurs() => _keepGoingWithChecks = true;

        [LoggerMessage(EventId = 130, Level = LogLevel.Error, Message = "Error in BacktrackingContinue: board.IsComplete()")]
        private static partial void LogError_BacktrackingContinue(ILogger<Board> logger, Exception ex);

        private bool BacktrackingContinue(Board board)
        {
            bool isComplete;
            try
            {
                isComplete = board.IsComplete();
            }
            catch (Exception ex)
            {
                LogError_BacktrackingContinue(_logger, ex);
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
                        BoardChangeEvent?.Invoke(newBoard, new SudokuEvent { Action = ECellAction.SetDigitInt, ChangedCellBase = newBoard[i] });
                        if (!result.Successful)
                        {
                            continue;
                        }

                        try
                        {
                            isComplete = newBoard.IsComplete();
                        }
                        catch (Exception ex)
                        {
                            LogError_BacktrackingContinue(_logger, ex);
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

        private void Cell_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => SomeChangesOccurs();

        private void Init()
        {
            _cells = new Cell[Consts.COUNTCELL];

            for (var i = 0; i < Consts.COUNTCELL; i++)
            {
                _cells[i] = new Cell(i);
                _cells[i].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
            }

            _history.Clear();

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

                _container[containerIdx] = new House[3];
                for (var containerType = 0; containerType < 3; containerType++)
                {
                    _container[containerIdx][containerType] = new House(fieldcontainer[containerType][containerIdx], (EHouseType)containerType, containerIdx);
                    foreach (var f in fieldcontainer[containerType][containerIdx])
                    {
                        f._fieldcontainters[containerType] = _container[containerIdx][containerType];
                    }
                }
            }
        }

        /// <summary>
        /// Adds a solve technique to the board.
        /// </summary>
        /// <param name="solveTechnique">The solve technique to add.</param>
        /// <returns>The board with the added solve technique.</returns>
        public Board AddSolveTechnique(ISolveTechnique<Cell> solveTechnique)
        {
            _solveTechniques ??= [];
            _solveTechniques.Add(solveTechnique);
            return this;
        }

        private void LoadSolveTechnics(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                LogWarning_PathNotFound(_logger, filePath);
                return;
            }

            var files = Directory.GetFiles(filePath, "*.dll");
            if (files.Length < 1)
            {
                LogWarning_PathNotFound(_logger, filePath);
                return;
            }

            _solveTechniques = new List<ISolveTechnique<Cell>>(files.Length);
            foreach (var file in files)
            {
                var st = SudokuSolveTechniqueLoader.LoadSolveTechnic<Cell>(file);
                _solveTechniques.Add(st);
            }
        }

        [LoggerMessage(EventId = 140, Level = LogLevel.Warning, Message = "LoadSolveTechnics: filePath not found: {filePath}")]
        private static partial void LogWarning_PathNotFound(ILogger<Board> logger, string filePath);
    }
}
