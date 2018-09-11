using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DE.Onnen.Sudoku
{
    [TestClass]
    public class SolveSomeExamplesTest
    {
        [TestMethod]
        public void TestHardestData()
        {
            IBoard board = new Board(new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad(), new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates(), new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad());
            string source = TestRessources.top95;
            int i = 0;
            for (i = 0; i < 2; i++)
            {
                IList<string> boards = source.Split('\n');

                int[] emh = new int[3];
                int total = 0;
                foreach (string line in boards)
                {
                    if (line.Length < 80)
                    {
                        continue;
                    }

                    total++;
                    string prueLine = "---";

                    try
                    {
                        prueLine = line.Substring(0, 81).Replace('.', '0');
                        board.SetCellsFromString(prueLine);
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Error in line{total} : {prueLine} " + ex.Message);
                        continue;
                    }
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
                            result = board.Backtracking();
                            if (!board.IsComplete || !result.Successful)
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
                source = TestRessources.HardestDatabase110626;
                board = new Board();
                Assert.AreEqual(total, emh.Sum(x => x));
                Assert.IsTrue(total > 10);
            }
            Assert.AreEqual(i, 2);
        }
    }
}