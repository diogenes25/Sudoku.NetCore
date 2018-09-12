using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;
using System;
using System.Linq;

namespace Soduko.Serialization
{
    public static class SudokuSerializer
    {
        public static string GetJson(this Board board, int cellid, int digit)
        {
            int[] actual = board.CreateSimpleBoard();
            string sLine = String.Join(',', actual);
            return $"{{{Environment.NewLine}\t\"cells\": [{sLine}],{Environment.NewLine}\t\"setdigit\": {{ \"id\": {cellid}, \"digit\": {digit} }}";
        }

        public static Board ParseToBoard(string json, params ASolveTechnique[] solveTechniques)
        {
            int firstBrck = json.IndexOf('[');
            int lastBrck = json.IndexOf(']');
            string cv = json.Substring(firstBrck + 1, lastBrck - firstBrck - 1);
            var cellval = cv.Split(',').Select(x => int.Parse(x.Trim())).ToArray();
            return new Board(cellval, solveTechniques);
        }
    }
}
