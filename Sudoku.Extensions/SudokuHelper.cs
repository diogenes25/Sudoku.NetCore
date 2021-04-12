using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DE.Onnen.Sudoku.Extensions
{
    public static class SudokuHelper
    {
        public static string PrintSudokuResult(SudokuLog sudokuResult)
        {
            var sb = new StringBuilder();
            PrintSudokuResult(sudokuResult, sb, "");
            return sb.ToString();
        }

        public static IList<string> ReadBoardFromFile(string file)
        {
            IList<string> retList = new List<string>();
            TextReader tr = new StreamReader(file);
            while (true)
            {
                tr.ReadLine();
                var sb = new StringBuilder();
                for (var y = 0; y < Consts.DIMENSIONSQUARE; y++)
                {
                    var line = tr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        tr.Close();
                        return retList;
                    }
                    sb.Append(line);
                }
                var sudokuLine = sb.ToString().Replace('.', '0');
                retList.Add(sudokuLine);
            }
        }

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
                if (line.Length < Consts.COUNTCELL)
                {
                    continue;
                }

                var sudokuLine = line.Replace('.', '0');
                retList.Add(sudokuLine);
            }
            tr.Close();
            return retList;
        }

        private static void PrintSudokuResult(SudokuLog sudokuResult, StringBuilder sb, string cap)
        {
            sb.Append(cap);
            sb.Append(sudokuResult.ToString());
            foreach (var sr in sudokuResult.ChildSudokuResult)
            {
                PrintSudokuResult(sr, sb, cap + " ");
            }
        }
    }
}
