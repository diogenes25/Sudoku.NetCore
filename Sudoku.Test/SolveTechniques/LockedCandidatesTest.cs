using DE.Onnen.Sudoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.SolveTechniques
{
	[TestClass]
	public class LockedCandidatesTest
	{
		/// <summary>
		///A test for Backtracking
		///</summary>
		[TestMethod()]
		public void Backtracking_solve_without_any_digit_and_LockedCandidates()
		{
			Board target = new Board(new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates());
			SudokuLog log = new SudokuLog();
			target.Backtracking(log);
			Assert.IsTrue(log.Successful);
			for (int i = 0; i < Consts.DimensionSquare; i++)
			{
				Assert.AreEqual((i + 1), target[i].Digit);
			}
		}
	}
}