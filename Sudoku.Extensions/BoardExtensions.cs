using System;
using System.Text;

namespace DE.Onnen.Sudoku.Extensions
{
    /// <summary>
    /// Extension-Methods for IBoard.
    /// </summary>
    /// <remarks>
    ///
    /// </remarks>
    public static class BoardExtensions
    {
        #region Public Methods

        public static string Matrix<C>(this IBoard<C> board)
        where C : ICell => Matrix(board, false);

        /// <summary>
        /// Nice Output.
        /// </summary>
        /// <remarks>
        /// &nbsp;&nbsp;123 456 789 <br/>
        /// &nbsp;┌───┬───┬───┐ <br/>
        /// A│579│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│<br/>
        /// B│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// C│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// &nbsp;├───┼───┼───┤ <br/>
        /// D│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// E│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// F│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// &nbsp;├───┼───┼───┤ <br/>
        /// G│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// H│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        /// I│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;│ <br/>
        ///  └───┴───┴───┘ <br/>
        /// Complete: 11,1111111111111 %
        /// </remarks>
        /// <param name="board"></param>
        /// <param name="onlyGiven"></param>
        /// <returns></returns>
        public static string Matrix<C>(this IBoard<C> board, bool onlyGiven)
        where C : ICell
        {
            // ╔═╦═╗
            // ║ ║ ║
            // ╠═╬═╣
            // ╚═╩═╝
            var sb = new StringBuilder();
            var id = 0;
            sb.Append("  123 456 789");
            sb.Append(Environment.NewLine);
            sb.Append(" ┌───┬───┬───┐");
            sb.Append(Environment.NewLine);
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    sb.Append(" ├───┼───┼───┤");
                    sb.Append(Environment.NewLine);
                }
                sb.Append((char)(i + 65));
                for (var x = 0; x < Consts.DIMENSIONSQUARE; x++)
                {
                    if (x % 3 == 0)
                    {
                        sb.Append('│');
                    }
                    if (onlyGiven)
                    {
                        sb.Append(((board[id].IsGiven) ? board[id].Digit.ToString() : " "));
                    }
                    else
                    {
                        sb.Append(((board[id].Digit > 0) ? board[id].Digit.ToString() : " "));
                    }
                    id++;
                }
                sb.Append('│');
                sb.Append(Environment.NewLine);
            }
            sb.Append(" └───┴───┴───┘");
            sb.Append(Environment.NewLine);
            sb.Append("Complete: ");
            sb.Append(board.SolvePercent);
            sb.Append(" %");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string MatrixWithCandidates<C>(this IBoard<C> board)
        where C : ICell
        {
            var sb = new StringBuilder();
            var lineID = 0;
            for (var boxX = 0; boxX < 3; boxX++)
            {
                sb.Append("┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐");
                sb.Append(Environment.NewLine);
                for (var boxY = 0; boxY < 3; boxY++)
                {
                    var cellIDY = 0;
                    for (int l = 0, m = (Consts.DIMENSION); l < m; l++)
                    {
                        for (int line = 0, maxline = (Consts.DIMENSION); line < maxline; line++)
                        {
                            sb.Append('│');
                            for (int partX = 0, maxpartX = (Consts.DIMENSION); partX < maxpartX; partX++)
                            {
                                for (int part = 0, maxpart = (Consts.DIMENSION); part < maxpart; part++)
                                {
                                    var digit = part + (cellIDY * 3) + 1;
                                    var cellID = (lineID * Consts.DIMENSIONSQUARE) + partX + (line * 3);
                                    string v;
                                    if (digit == 5 && board[cellID].Digit > 0)
                                    {
                                        v = board[cellID].Digit.ToString();
                                    }
                                    else
                                    {
                                        v = (board[cellID].Candidates.Contains(digit)) ? digit.ToString() : " ";
                                    }
                                    sb.Append(v);
                                }
                                sb.Append('│');
                            }
                        }
                        sb.Append(Environment.NewLine);
                        cellIDY++;
                    }
                    lineID++;
                    if (boxY < 2)
                    {
                        sb.Append("├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤");
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("└───┴───┴───┘└───┴───┴───┘└───┴───┴───┘");
                        sb.Append(Environment.NewLine);
                    }
                }
            }
            sb.Append(Environment.NewLine);
            sb.Append("Complete: ");
            sb.Append(board.SolvePercent);
            sb.Append(" %");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static void SetCellsFromString<C>(this IBoard<C> board, string line)
                                where C : ICell => SetCellsFromString(board, line, '0');

        public static void SetCellsFromString<C>(this IBoard<C> board, string line, char zero)
        where C : ICell
        {
            board.Clear();
            var max = Consts.COUNTCELL;
            if (line.Length < max)
            {
                throw new ArgumentOutOfRangeException("string is to short");
            }

            for (var x = 0; x < max; x++)
            {
                var currChar = line[x];
                if (!currChar.Equals(zero))
                {
                    var digit = Convert.ToInt32(currChar) - 48;
                    var result = board.SetDigit(x, digit);
                    if (!result.Successful)
                    {
                        throw new InvalidOperationException($"Digit : {digit} could not be set");
                    }
                }
            }
        }

        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="row">Row Range from 0-8 or 'A'-'I' or 'a'-'i'</param>
        /// <param name="col">Column</param>
        /// <param name="digit">Digit</param>
        public static SudokuLog SetDigit<C>(this IBoard<C> board, int row, int col, int digit) where C : ICell
        {
            var currentRow = row;
            var lowRow = (int)'A'; // 65
            if (row >= lowRow) // If row is greater or equal than 65 (ASCII of 'A') the row-value could be a char instead of an int.
            {
                currentRow = (int)char.ToUpper((char)row);
                currentRow -= lowRow;
            }

            var sudokuResult = new SudokuLog
            {
                EventInfoInResult = new SudokuEvent
                {
                    ChangedCellBase = null,
                    Action = CellAction.SetDigitInt,
                    SolveTechnik = "SetDigit",
                }
            };

            if (currentRow < 0 || currentRow > Consts.DIMENSIONSQUARE)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = $"row must be between 1 and {Consts.DIMENSIONSQUARE} or between 'a' and '{((char)(lowRow + Consts.DIMENSIONSQUARE))}'";
                return sudokuResult;
            }

            if (col < 0 || col > Consts.DIMENSIONSQUARE)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = $"col must be between 0 and '{Consts.DIMENSIONSQUARE - 1}'";
                return sudokuResult;
            }

            if (digit < 1 || digit > Consts.DIMENSIONSQUARE)
            {
                sudokuResult.Successful = false;
                sudokuResult.ErrorMessage = $"digit must be between 0 and '{Consts.DIMENSIONSQUARE - 1}'";
                return sudokuResult;
            }

            return board.SetDigit((currentRow * Consts.DIMENSIONSQUARE) + col, digit);
        }

        public static string ToHtmlTable<C>(this IBoard<C> board) where C : ICell => ToHtmlTable(board, false);

        public static string ToHtmlTable<C>(this IBoard<C> board, bool onlyGiven) where C : ICell
        {
            var sb = new StringBuilder();
            var id = 0;
            sb.Append("<table class=\"sudokutbl\">");
            sb.Append(Environment.NewLine);
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
            {
                sb.Append(Environment.NewLine);
                sb.Append("<tr class=\"sudokurow\">");
                sb.Append(Environment.NewLine);
                sb.Append('\t');
                for (var x = 0; x < Consts.DIMENSIONSQUARE; x++)
                {
                    sb.Append($"<td class=\"{((board[id].IsGiven) ? "sudokucell_given" : "sudokucell")}\" id=\"cell[");
                    sb.Append(id);
                    sb.Append("]\" >");
                    if (onlyGiven)
                    {
                        sb.Append(((board[id].IsGiven) ? board[id].Digit.ToString() : "&nbsp;"));
                    }
                    else
                    {
                        sb.Append(((board[id].Digit > 0) ? board[id].Digit.ToString() : "[" + string.Join('|', board[id].Candidates) + "]" + board[id].CandidateValue));
                    }
                    sb.Append("</td>");
                    id++;
                }
                sb.Append(Environment.NewLine);
                sb.Append("</tr>");
            }
            sb.Append(Environment.NewLine);
            sb.Append("</table>");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        #endregion Public Methods
    }
}
