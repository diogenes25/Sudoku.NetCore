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
        public static void SetCellsFromString(this IBoard board, string line)
        {
            SetCellsFromString(board, line, '0');
        }

        public static void SetCellsFromString(this IBoard board, string line, char zero)
        {
            board.Clear();
            int max = Consts.CountCell;
            if (line.Length < max)
            {
                throw new ArgumentOutOfRangeException("string is to short");
            }

            for (int x = 0; x < max; x++)
            {
                char currChar = line[x];
                if (!currChar.Equals(zero))
                {
                    int digit = Convert.ToInt32(currChar) - 48;
                    SudokuLog result = board.SetDigit(x, digit);
                    if (!result.Successful)
                    {
                        throw new InvalidOperationException("Digit : " + digit + " could not be set");
                    }
                }
            }
        }

        public static string Matrix(this IBoard board)
        {
            return Matrix(board, false);
        }

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
        public static string Matrix(this IBoard board, bool onlyGiven)
        {
            // ╔═╦═╗
            // ║ ║ ║
            // ╠═╬═╣
            // ╚═╩═╝
            StringBuilder sb = new StringBuilder();
            int id = 0;
            sb.Append("  123 456 789");
            sb.Append(Environment.NewLine);
            sb.Append(" ┌───┬───┬───┐");
            sb.Append(Environment.NewLine);
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    sb.Append(" ├───┼───┼───┤");
                    sb.Append(Environment.NewLine);
                }
                IHouse house = board.GetHouse(HouseType.Row, i);
                sb.Append((char)(i + 65));
                for (int x = 0; x < Consts.DimensionSquare; x++)
                {
                    if (x % 3 == 0)
                    {
                        sb.Append("│");
                    }
                    if (onlyGiven)
                    {
                        sb.Append(((board[id].IsGiven) ? board[id].Digit.ToString() : " "));
                    }
                    else
                    {
                        sb.Append(((house[x].Digit > 0) ? house[x].Digit.ToString() : " "));
                    }
                    id++;
                }
                sb.Append("│");
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

        public static string MatrixWithCandidates(this IBoard board)
        {
            StringBuilder sb = new StringBuilder();
            int LineID = 0;
            for (int boxX = 0; boxX < 3; boxX++)
            {
                sb.Append("┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐");
                sb.Append(Environment.NewLine);
                for (int boxY = 0; boxY < 3; boxY++)
                {
                    int cellIDY = 0;
                    for (int l = 0, m = (Consts.Dimension); l < m; l++)
                    {
                        for (int line = 0, maxline = (Consts.Dimension); line < maxline; line++)
                        {
                            sb.Append("│");
                            for (int partX = 0, maxpartX = (Consts.Dimension); partX < maxpartX; partX++)
                            {
                                for (int part = 0, maxpart = (Consts.Dimension); part < maxpart; part++)
                                {
                                    int digit = part + (cellIDY * 3) + 1;
                                    int cellID = (LineID * Consts.DimensionSquare) + partX + (line * 3);
                                    string v = String.Empty;
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
                                sb.Append("│");
                            }
                        }
                        sb.Append(Environment.NewLine);
                        cellIDY++;
                    }
                    LineID++;
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

        public static string ToHtmlTable(this IBoard board)
        {
            return ToHtmlTable(board, false);
        }

        public static string ToHtmlTable(this IBoard board, bool onlyGiven)
        {
            StringBuilder sb = new StringBuilder();
            int id = 0;
            sb.Append("<table class=\"sudokutbl\">");
            sb.Append(Environment.NewLine);
            for (int i = 0; i < Consts.DimensionSquare; i++)
            {
                sb.Append(Environment.NewLine);
                sb.Append("<tr class=\"sudokurow\">");
                sb.Append(Environment.NewLine);
                sb.Append("\t");
                for (int x = 0; x < Consts.DimensionSquare; x++)
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
                        sb.Append(((board[id].Digit > 0) ? board[id].Digit.ToString() : "[" + String.Join('|', board[id].Candidates) + "]" + board[id].CandidateValue));
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

        /// <summary>
        /// Set a digit at cell.
        /// </summary>
        /// <param name="row">Row Range from 0-8 or 'A'-'I' or 'a'-'i'</param>
        /// <param name="col">Column</param>
        /// <param name="digit">Digit</param>
        public static SudokuLog SetDigit(this IBoard board, int row, int col, int digit)
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
                EventInfoInResult = new SudokuEvent
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

            cellid = (currentRow * Consts.DimensionSquare) + col;

            return board.SetDigit(cellid, digit);
        }
    }
}