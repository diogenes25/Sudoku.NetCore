using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.Extensions.Logging;

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
        private Cell[] _cells = new Cell[Consts.COUNTCELL];

        private const int BLOCK_CONTAINERTYPE = 2;
        private const int COL_CONTAINERTYPE = 1;
        private const int ROW_CONTAINERTYPE = 0;
        private readonly House[][] _container = new House[Consts.DIMENSIONSQUARE][];
        private readonly List<SudokuHistoryItem> _history = new();
        private bool _keepGoingWithChecks;
        private List<ISolveTechnique<Cell>>? _solveTechniques;
        private readonly ILogger<Board>? _logger;

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

        public Board(ILogger<Board>? logger)
        {
            _logger = logger;
            Init();
        }

        public Board(IEnumerable<ISolveTechnique<Cell>> solveTechniques, ILogger<Board>? logger)
        {
            _solveTechniques = solveTechniques?.ToList();
            _logger = logger;
            Init();
        }

        public Board(string filePath)
        {
            LoadSolveTechnics(filePath);
            Init();
        }

        public Board([NotNull] IEnumerable<int> uniqueCellIDs, IEnumerable<ISolveTechnique<Cell>>? solveTechniques = null, ILogger<Board>? logger = null)
        {
            _solveTechniques = solveTechniques?.ToList();
            _logger = logger;
            Init();
            FillBoardWithUniqueCellIDs(uniqueCellIDs);
        }

        /// <summary>
        /// Changes in board
        /// </summary>
        public event System.EventHandler<SudokuEvent> BoardChangeEvent;

        /// <summary>
        /// Count of cells (normaly 81 = 9*9)
        /// </summary>
        public int Count => _cells.Length;

        /// <summary>
        /// History of Steps during the solvingprocess
        /// </summary>
        public ReadOnlyCollection<SudokuHistoryItem> History => _history.AsReadOnly();

        /// <summary>
        /// Percent
        /// </summary>
        /// <returns>Percent</returns>
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
        /// Loaded solvetechniques.
        /// </summary>
        public IList<ISolveTechnique<Cell>>? SolveTechniques => _solveTechniques?.Select(x => (ISolveTechnique<Cell>)x).ToList<ISolveTechnique<Cell>>();

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

        public IHouse<Cell> GetHouse(EHouseType houseType, int houseID) => _container[houseID][(int)houseType];

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

        /// <inheritdoc/>
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
                    Action = ECellAction.SetDigitInt,
                    SolveTechnique = "SetDigit",
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
                if (_history.Count > 0)
                {
                    SetHistory(_history.Count - 1);
                }
                _cells[cellID].RemoveCandidate(digitToSet, sudokuResult);
            }
            return sudokuResult;
        }

        private void SetHistory(int historyId)
        {
            if (historyId < 0 || historyId >= _history.Count)
            {
                _logger?.LogWarning($"SetHistory with parameter historyId (value:{historyId}) is out of range (0 - {_history.Count})");
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
                        _logger?.LogError(ex, $"Error in SetHistory: _cells[{i}].Digit = _history[{historyId}].BoardInt[{i}]");
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
        public SudokuLog StartSolve()
        {
            var initSudokuLog = new SudokuLog();
            Solve(initSudokuLog);
            return initSudokuLog;
        }

        /// <summary>
        /// Solves Sudoku with SolveTechniques (no Backtracking).
        /// </summary>
        /// <param name="sudokuResult">Log</param>
        private void Solve(SudokuLog sudokuResult)
        {
            var tmpSudokuResult = sudokuResult;
            tmpSudokuResult ??= new SudokuLog();

            if (_solveTechniques is null || _solveTechniques.Count < 1)
            {
                sudokuResult.Successful = true;
                sudokuResult.ErrorMessage = "No SolveTechnique is set";
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

        private bool BacktrackingContinue(Board board)
        {
            bool isComplete;
            try
            {
                isComplete = board.IsComplete();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in BacktrackingContinue: board.IsComplete()");
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
                            _logger?.LogError(ex, "Error in BacktrackingContinue: board.IsComplete()");
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

        public Board AddSolveTechnique(ISolveTechnique<Cell> solveTechnique)
        {
            _solveTechniques ??= new List<ISolveTechnique<Cell>>();
            _solveTechniques.Add(solveTechnique);
            return this;
        }

        private void LoadSolveTechnics(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger?.LogWarning("LoadSolveTechnics: filePath is not set.");
                return;
            }

            var files = Directory.GetFiles(filePath, "*.dll");
            if (files.Length < 1)
            {
                _logger?.LogWarning($"LoadSolveTechnics: filePath not found: {filePath}");
                return;
            }

            _solveTechniques = new List<ISolveTechnique<Cell>>(files.Length);
            foreach (var file in files)
            {
                var st = SudokuSolveTechniqueLoader<Cell>.LoadSolveTechnic(file);
                _solveTechniques.Add(st);
            }
        }
    }
}
