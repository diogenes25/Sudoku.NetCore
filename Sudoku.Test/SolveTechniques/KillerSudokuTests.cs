using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques.KillerSudoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test.SolveTechniques
{
    [TestClass]
    public class KillerSudokuTests
    {
        [TestMethod]
        public void Create_KillerBox()
        {
            var killer = new KillerSudokuTEchnique<Cell>();
            var cell = killer.AddHouse(5, 0, 0).Start().Right().Right().Down();
            Assert.IsNotNull(cell);
            Assert.AreEqual(0, cell.Leafs());
            Assert.AreEqual(1, cell.ParentCell.Leafs());
            Assert.AreEqual(2, cell.ParentCell.ParentCell.Leafs());
            cell.ParentCell.Right();
            Assert.AreEqual(2, cell.ParentCell.Leafs());
            Assert.AreEqual(3, cell.ParentCell.ParentCell.Leafs());
        }

        [TestMethod]
        public void House_KillerBox()
        {
            var killer = new KillerSudokuTEchnique<Cell>();
            var house = killer.AddHouse(6, 0, 0);
            var cell1 = house.Start();
            Assert.IsNotNull(cell1);
            Assert.AreEqual(0, cell1.Leafs());
            house.Start().Down().Down().Right();
            Assert.AreEqual(3, cell1.Leafs());
            cell1.Right();
            Assert.AreEqual(4, cell1.Leafs());
            cell1.Right();
            Assert.AreEqual(4, cell1.Leafs());
            //Assert.AreEqual(2, cell.ParentCell.ParentCell.Leafs());
            //cell.ParentCell.Right();
            //Assert.AreEqual(2, cell.ParentCell.Leafs());
            //Assert.AreEqual(3, cell.ParentCell.ParentCell.Leafs());
        }

        [TestMethod]
        public void TTTT()
        {
            var anz = 3;
            var erg = 8;
            var minBorder = 0;
            var i = 1;
            for (i = 1; i < anz; i++) { minBorder += i; }
            Assert.AreEqual(3, minBorder);
            var max = erg - minBorder;
            var min = 1;
            if (max > 9)
            {
                max = 9;
                min = minBorder;
            }
            Assert.AreEqual(5, max);
            Assert.AreEqual(1, min);

            erg = 16; // 3,4,9
            max = erg - minBorder;
            min = 1;
            if (max > 9)
            {
                max = 9;
                min = minBorder;
            }
            Assert.AreEqual(9, max);
            Assert.AreEqual(3, min);

            erg = 18; // 4,5,9
            max = erg - minBorder;
            min = 1;
            if (max > 9)
            {
                var d = erg - minBorder - 9;
                max = erg - minBorder;
                min = minBorder;
            }
            // Assert.AreEqual(9, max);
            // Assert.AreEqual(4, min);

            var sss = new Dictionary<int, Dictionary<int, List<int>>>();
            sss.Add(1, new Dictionary<int, List<int>> { { 1, new List<int> { 1 } } });
            sss.Add(2, new Dictionary<int, List<int>> { { 1, new List<int> { 2 } } });
            sss.Add(3, new Dictionary<int, List<int>> { { 1, new List<int> { 3 } }, { 2, new List<int> { 1, 2 } } });
            var cand = sss[3][2]; // House-Value: 3 Cell-Count: 2
            Assert.AreEqual(cand.Count, 2);

            for (var anzCells = 2; anzCells <= 9; anzCells++)
            {
                for (var startDigit = 1; startDigit <= anzCells; startDigit++)
                {
                    for (var endDigit = 9; endDigit > startDigit; endDigit--)
                    {

                        Console.WriteLine($"anzCells:{anzCells}, startDigit{startDigit}, endDigit:{endDigit} = ");
                        //for (var endDigit = startDigit; endDigit < 9-anzCells; endDigit++)
                        //{
                        //    Console.Write($"anzCells:{anzCells}, startDigit{startDigit}, endDigit:{endDigit} = ");
                        //    var list = Enumerable.Range(startDigit, endDigit).ToList();
                        //    Console.WriteLine(string.Join(",", list));
                        //}

                    }

                }
            }
        }
    }
}
