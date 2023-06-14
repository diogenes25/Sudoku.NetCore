//-----------------------------------------------------------------------
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
        #region Public Methods

        /// <summary>
        /// Test Sudoku that can only be beaten with backtracking
        /// </summary>
        [TestMethod]
        public void TestHardestData_Test()
        {
            var board = new Board(new SolveTechniques.HiddenPairTripleQuad<Cell>(), new SolveTechniques.LockedCandidates<Cell>(), new SolveTechniques.NakedPairTrippleQuad<Cell>());
            var i = 0;
            for (i = 0; i < 2; i++)
            {
                var source = TestResource.top95;
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
                        currentLine = line[..81].Replace('.', '0');
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
                        var result = board.StartSolve();
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

                source = TestResource.HardestDatabase110626;
                board.Clear();
                Assert.AreEqual(total, emh.Sum(x => x));
                Assert.IsTrue(total > 10);
            }

            Assert.AreEqual(i, 2);
        }

        #endregion Public Methods
    }
}
