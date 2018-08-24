using DE.Onnen.Sudoku.SolveTechniques;
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
	public class Board : ACellCollection, ICloneable, IBoard, ISudokuHost
	{
		private const int ROW_CONTAINERTYPE = 0;
		private const int COL_CONTAINERTYPE = 1;
		private const int BLOCK_CONTAINERTYPE = 2;
		private static readonly int countCell = Consts.DimensionSquare * Consts.DimensionSquare;

		private List<SudokuHistoryItem> history;

		//private List<ICell> givens;
		private readonly House[][] container = new House[Consts.DimensionSquare][];

		private double solvePercentBase = 0;
		private ASolveTechnique[] _solveTechniques;

		public ReadOnlyCollection<SudokuHistoryItem> History { get { return this.history.AsReadOnly(); } }

		/// <inheritdoc />
		public ReadOnlyCollection<ICell> Givens { get { return this._cells.Where(x => x.IsGiven).Select(x => (ICell)x).ToList().AsReadOnly(); } }

		/// <summary>
		/// Loaded solvetechniques.
		/// </summary>
		public IList<ISolveTechnique> SolveTechniques { get { return this._solveTechniques.Select(x => (ISolveTechnique)x).ToList<ISolveTechnique>(); } }

		public IHouse GetHouse(HouseType houseType, int idx)
		{
			return this.container[idx][(int)houseType];
		}

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
					currSolvePercent += (Consts.DimensionSquare - c.Candidates.Count);
				}

				return (currSolvePercent / this.solvePercentBase) * 100;
			}
		}

		/// <inheritdoc />
		public bool IsComplete
		{
			get
			{
				int containerType = 0;
				for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
				{
					if (!this.container[containerIdx][containerType].IsComplete)
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Some changes happend while solving. Another check is needed.
		/// </summary>
		internal void ReCheck()
		{
			this.reCheck = true;
		}

		private bool reCheck = false;

		public Board(params ASolveTechnique[] solveTechniques)
		{
			Init();
			this._solveTechniques = solveTechniques;
			if (this._solveTechniques != null && this._solveTechniques.Length > 0)
			{
				foreach (ASolveTechnique st in this._solveTechniques)
				{
					st.SetBoard(this);
				}
			}
		}

		public Board()
		//: this("..\\..\\..\\Sudoku\\SolveTechnics\\")
		{
			Init();
		}

		public Board(string filePath)
		{
			Init();
			LoadSolveTechnics(filePath);
		}

		private void Init()
		{
			this._cells = new Cell[Consts.DimensionSquare * Consts.DimensionSquare];
			//this.givens = new List<ICell>();
			this.history = new List<SudokuHistoryItem>();
			this.solvePercentBase = Math.Pow(Consts.DimensionSquare, 3.0);
			for (int i = 0; i < Consts.DimensionSquare * Consts.DimensionSquare; i++)
			{
				this._cells[i] = new Cell(i);
				this._cells[i].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cell_PropertyChanged);
			}

			ICell[][][] fieldcontainer;
			fieldcontainer = new ICell[3][][];
			fieldcontainer[ROW_CONTAINERTYPE] = new ICell[Consts.DimensionSquare][]; // Row
			fieldcontainer[COL_CONTAINERTYPE] = new ICell[Consts.DimensionSquare][]; // Col
			fieldcontainer[BLOCK_CONTAINERTYPE] = new ICell[Consts.DimensionSquare][]; // Block

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

				this.container[containerIdx] = new House[3];
				for (int containerType = 0; containerType < 3; containerType++)
				{
					this.container[containerIdx][containerType] = new House(fieldcontainer[containerType][containerIdx], (HouseType)containerType, containerIdx);
					foreach (Cell f in fieldcontainer[containerType][containerIdx])
					{
						f._fieldcontainters[containerType] = this.container[containerIdx][containerType];
					}
				}
			}
		}

		private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.ReCheck();
		}

		/// <summary>
		/// Set a digit at cell.
		/// </summary>
		/// <param name="row">Row Range from 0-8 or 'A'-'I' or 'a'-'i'</param>
		/// <param name="col">Column</param>
		/// <param name="digit">Digit</param>
		public SudokuLog SetDigit(int row, int col, int digit)
		{
			int cellid = 0;
			int currentRow = row;
			int lowRow = (int)'A'; // 65
			if (row >= lowRow) // If row is greater or equal than 65 (ASCII of 'A') the row-value could be a char instead of an int.
			{
				currentRow = (int)Char.ToUpper((char)row);
				currentRow -= lowRow;
			}

			SudokuLog sudokuResult = new SudokuLog
			{
				EventInfoInResult = new SudokuEvent()
				{
					ChangedCellBase = null,
					Action = CellAction.SetDigitInt,
					SolveTechnik = "SetDigit",
				}
			};

			if (currentRow < 0 || currentRow > Consts.DimensionSquare)
			{
				sudokuResult.Successful = false;
				sudokuResult.ErrorMessage = $"row must be between 1 and {Consts.DimensionSquare} or between 'a' and '{((char)(lowRow + Consts.DimensionSquare))}'";
				return sudokuResult;
			}

			if (col < 0 || col > Consts.DimensionSquare)
			{
				sudokuResult.Successful = false;
				sudokuResult.ErrorMessage = $"col must be between 0 and '{Consts.DimensionSquare - 1}'";
				return sudokuResult;
			}

			if (digit < 1 || digit > Consts.DimensionSquare)
			{
				sudokuResult.Successful = false;
				sudokuResult.ErrorMessage = $"digit must be between 0 and '{Consts.DimensionSquare - 1}'";
				return sudokuResult;
			}

			checked
			{
				cellid = (currentRow * Consts.DimensionSquare) + col;
			}

			return this.SetDigit(cellid, digit);

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
				EventInfoInResult = new SudokuEvent()
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
				//this.givens.Add(this.cells[cellID]);
				if (withSolve)
				{
					this.Solve(sudokuResult);
				}
			}

			if (sudokuResult.Successful)
			{
				this.history.Add(new SudokuHistoryItem(this, this._cells[cellID], sudokuResult));
			}
			else
			{
				SetHistory(this.history.Count - 1);
				this._cells[cellID].RemoveCandidate(digitToSet, sudokuResult);
				//if (withSolve)
				//{
				//    this.Solve(sudokuResult);
				//}
			}
			return sudokuResult;
		}

		/// <summary>
		/// Set Cells from otherBoard to this Board.
		/// </summary>
		/// <param name="otherBoard">Another Board</param>
		public void SetBoard(IBoard otherBoard)
		{
			if (otherBoard == null || otherBoard == null)
			{
				return;
			}

			for (int i = 0; i < otherBoard.Count; i++)
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

		public void SetHistory(int historyId)
		{
			if (historyId < 0 || historyId >= this.history.Count)
				return;
			for (int i = 0; i < this.history[historyId].BoardInt.Length; i++)
			{
				if (this.history[historyId].BoardInt[i] < 0)
				{
					try
					{
						this._cells[i].Digit = this.history[historyId].BoardInt[i] * -1;
					}
					catch
					{
						//Console.WriteLine(ex.Message);
						continue;
					}
				}
				else
				{
					this._cells[i].CandidateValue = this.history[historyId].BoardInt[i];
				}
			}

			//this.Solve(new SudokuResult());
		}

		/// <summary>
		/// Solves Sudoku with SolveTechniques (no Backtracking).
		/// </summary>
		/// <param name="sudokuResult">Log</param>
		public void Solve(SudokuLog sudokuResult)
		{
			if (sudokuResult == null)
			{
				sudokuResult = new SudokuLog();
			}

			do
			{
				this.reCheck = false;
				for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
				{
					for (int containerType = 0; containerType < 3; containerType++)
					{
						if (this.container[containerIdx][containerType].IsComplete)
							continue;
						if (this._solveTechniques != null && this._solveTechniques.Length > 0)
						{
							foreach (ASolveTechnique st in this._solveTechniques)
							{
								if (st.IsActive)
								{
									if (!this.container[containerIdx][containerType].ReCheck && st.CellView == ECellView.OnlyHouse)
										continue;
									st.SolveHouse(this.container[containerIdx][containerType], sudokuResult);
									if (!sudokuResult.Successful)
										return;
								}
							}
						}
						this.container[containerIdx][containerType].ReCheck = false;
					}
				}
			} while (this.reCheck);
		}

		public void Backtracking()
		{
			this.Backtracking(new SudokuLog());
		}

		public void Backtracking(SudokuLog sudokuResult)
		{
			if (sudokuResult == null)
			{
				sudokuResult = new SudokuLog()
				{
					Successful = false,
					ErrorMessage = "SudokuLog is null"
				};
			}
			else if (!BacktrackingContinue((Board)this.Clone()))
			{
				sudokuResult.Successful = false;
				sudokuResult.ErrorMessage = "Sudoku hat keine Lösung";
			}
		}

		/// <summary>
		/// Reset Board.
		/// </summary>
		public new void Clear()
		{
			this.history.Clear();
			base.Clear();

			for (int containerIdx = 0; containerIdx < Consts.DimensionSquare; containerIdx++)
			{
				for (int containerType = 0; containerType < 3; containerType++)
				{
					this.container[containerIdx][containerType].CandidateValue = (1 << Consts.DimensionSquare) - 1;
				}
			}
		}

		private bool BacktrackingContinue(Board board)
		{
			if (board.IsComplete)
				return true;

			for (int i = 0; i < this._cells.Length; i++)
			{
				if (board[i].Digit == 0)
				{
					ReadOnlyCollection<int> posDigit = board[i].Candidates;
					foreach (int x in posDigit)
					//System.Threading.Tasks.Parallel.ForEach(posDigit, x=>
					{
						Board newBoard = (Board)board.Clone();
						SudokuLog result = newBoard.SetDigit(i, x, true);
						BoardChangeEvent?.Invoke(newBoard, new SudokuEvent() { Action = CellAction.SetDigitInt, ChangedCellBase = newBoard[i] });
						//Thread.Sleep(300);
						if (!result.Successful)
						{
							//Console.WriteLine(board.Cells[i].ToString() + " X SetDigit " + x);
							continue;
						}
						if (newBoard.IsComplete)
						{
							//newBoard.Show();
							for (int s = 0; s < this._cells.Length; s++)
							{
								this._cells[s].Digit = newBoard._cells[s].Digit;
								//this.cells[s].BaseValue = 0;
							}
							return true;
						}
						if (BacktrackingContinue(newBoard))
						{
							//Console.WriteLine("OK " + board.Cells[i].ToString() + " : " + x);
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
			Board cloneboard = new Board(this._solveTechniques);

			for (int i = 0; i < this._cells.Length; i++)
			{
				if (this._cells[i].Digit > 0)
					cloneboard._cells[i].Digit = this._cells[i].Digit;
				else
					cloneboard._cells[i].CandidateValue = this._cells[i].CandidateValue;
			}

			return cloneboard;
		}

		#endregion ICloneable Members

		private void LoadSolveTechnics(string filePath)
		{
			if (String.IsNullOrWhiteSpace(filePath))
				return;

			List<string> files = new List<string>(Directory.GetFiles(filePath, "*.dll"));
			if (files == null || files.Count() < 1)
				return;

			this._solveTechniques = new ASolveTechnique[files.Count()];
			int fileCount = 0;
			foreach (string file in files)
			{
				ASolveTechnique st = SudokuSolveTechniqueLoader.LoadSolveTechnic(file, this);
				this._solveTechniques[fileCount++] = st;
				st.SetBoard(this);
			}
		}

		#region ISudokuHost Members

		public void Register(ISolveTechnique solveTechnic)
		{
		}

		#endregion ISudokuHost Members

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ICell cell in this)
			{
				sb.Append(cell.Digit);
			}
			return sb.ToString();
		}


		/// <summary>
		/// Convert a Board to a int-Array
		/// </summary>
		/// <remarks>
		/// Positiv value = Candidates as bitmask.<br/>
		/// Negativ value = Digit.
		/// </remarks>
		/// <param name="board"></param>
		/// <returns></returns>
		public static int[] CreateSimpleBoard(IBoard board)
		{
			if (board == null || board.Count < 1)
			{
				return null;
			}

			int[] retLst = new int[countCell];
			for (int i = 0; i < board.Count; i++)
			{
				if (board[i].Digit > 0)
				{
					retLst[i] = board[i].Digit * -1;
				}
				else
				{
					retLst[i] = board[i].CandidateValue;
				}
			}
			return retLst;
		}

		public override bool Equals(object obj)
		{
			bool retVal = false;
			if (obj != null && obj is IBoard)
			{
				IBoard nb = (IBoard)obj;
				retVal = true;
				for (int i = 0; i < Consts.DimensionSquare; i++)
				{
					retVal &= this[i].Digit == nb[i].Digit;
				}
			}
			return retVal;
		}

		public override int GetHashCode()
		{
			int retHash = 0;
			foreach (ICell c in this)
			{
				retHash += (c.Digit + (1 << (c.ID % 9)));
			}
			return retHash;
		}
	}
}