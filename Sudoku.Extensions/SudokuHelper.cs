using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DE.Onnen.Sudoku.Extensions
{
    public static class SudokuHelper
    {
        private static readonly int countCell = Consts.DimensionSquare * Consts.DimensionSquare;

        /// <summary>
        /// Read Sudokus from file.
        /// </summary>
        /// <remarks>format:<br/>
        /// 123456789123456789123456789123456789 usw
        /// </remarks>
        /// <param name="file">Path/File to textfile</param>
        /// <returns>List of Boards</returns>
        public static IList<string> ReadBoardFromFileTop(string file)
        {
            IList<string> retList = new List<string>();
            TextReader tr = new StreamReader(file);
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                if (line.Length < countCell)
                {
                    continue;
                }

                string sudokuLine = line.Replace('.', '0');
                retList.Add(sudokuLine);
            }
            tr.Close();
            return retList;
        }

        public static IList<string> ReadBoardFromFile(string file)
        {
            IList<string> retList = new List<string>();
            TextReader tr = new StreamReader(file);
            while (true)
            {
                tr.ReadLine();
                StringBuilder sb = new StringBuilder();
                for (int y = 0; y < Consts.DimensionSquare; y++)
                {
                    string line = tr.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                    {
                        tr.Close();
                        return retList;
                    }
                    sb.Append(line);
                }
                string sudokuLine = sb.ToString().Replace('.', '0');
                retList.Add(sudokuLine);
            }
        }

        public static string PrintSudokuResult(SudokuLog sudokuResult)
        {
            StringBuilder sb = new StringBuilder();
            PrintSudokuResult(sudokuResult, sb, "");
            return sb.ToString();
        }

        private static void PrintSudokuResult(SudokuLog sudokuResult, StringBuilder sb, string cap)
        {
            sb.Append(cap);
            sb.Append(sudokuResult.ToString());
            foreach (SudokuLog sr in sudokuResult.ChildSudokuResult)
            {
                PrintSudokuResult(sr, sb, cap + " ");
            }
        }
    }
}