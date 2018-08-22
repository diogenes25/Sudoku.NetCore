using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku.Test;
using System;
using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
	[TestClass()]
	public class SolveSomeExamplesTest
	{

		[TestMethod()]
		public void TestHardestData()
		{

			IList<string> boards = TestRessources.HardestDatabase110626.Split(Environment.NewLine);
			IBoard board = new Board(new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad(), new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates(), new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad());

			int[] emh = new int[3];
			foreach (string line in boards)
			{
				board.SetCellsFromString(line);
				DateTime start = DateTime.Now;
				if (board.IsComplete)
				{
					emh[0] += 1;
				}
				else
				{
					SudokuLog result = new SudokuLog();
					board.Solve(result);
					if (board.IsComplete)
					{
						emh[1] += 1;
					}
					else
					{
						board.Backtracking(result);
						if (!board.IsComplete)
						{
							Assert.Fail("Board is not solved");
						}
						else
						{
							emh[2] += 1;
						}
					}
				}
			}
		}
	}
}
