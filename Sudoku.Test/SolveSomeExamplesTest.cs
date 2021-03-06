﻿//-----------------------------------------------------------------------
// <copyright file="SolveSomeExamplesTest.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DE.Onnen.Sudoku.Extensions;
    using global::Sudoku.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test some real world examples.
    /// </summary>
    [TestClass]
    public class SolveSomeExamplesTest
    {
        /// <summary>
        /// Test Sudoku that can only be beaten with backtracking
        /// </summary>
        [TestMethod]
        public void TestHardestData_Test()
        {
            IBoard<Cell> board = new Board(new DE.Onnen.Sudoku.SolveTechniques.HiddenPairTripleQuad<Cell>(), new DE.Onnen.Sudoku.SolveTechniques.LockedCandidates<Cell>(), new DE.Onnen.Sudoku.SolveTechniques.NakedPairTrippleQuad<Cell>());

            var source = TestRessources.top95;
            var i = 0;
            for (i = 0; i < 2; i++)
            {
                IList<string> boards = source.Split('\n');

                var emh = new int[3];
                var total = 0;
                foreach (var line in boards)
                {
                    if (line.Length < 81)
                    {
                        continue;
                    }

                    total++;
                    var currentLine = "---";

                    try
                    {
                        currentLine = line.Substring(0, 81).Replace('.', '0');
                        board.SetCellsFromString(currentLine);
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail($"Error in line{total} : {currentLine} " + ex.Message);
                        continue;
                    }

                    if (board.IsComplete())
                    {
                        emh[0] += 1;
                    }
                    else
                    {
                        var result = new SudokuLog();
                        board.Solve(result);
                        if (board.IsComplete())
                        {
                            emh[1] += 1;
                        }
                        else
                        {
                            result = board.Backtracking();
                            if (!board.IsComplete() || !result.Successful)
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
