﻿using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    ///This is a test class for CellTest and is intended
    ///to contain all CellTest Unit Tests
    ///</summary>
    [TestClass]
    public class CellTest
    {
        /// <summary>
        ///A test for Candidates
        ///</summary>
        [TestMethod]
        public void Candidates_are_9_Test()
        {
            var id = 0;
            var target = new Cell(id);
            ReadOnlyCollection<int> actual;
            actual = target.Candidates;
            Assert.AreEqual(Consts.DimensionSquare, actual.Count);
            for (var i = 0; i < Consts.DimensionSquare; i++)
            {
                Assert.AreEqual(actual[i], i + 1);
            }
        }

        /// <summary>
        ///A test for Candidates
        ///</summary>
        [TestMethod]
        public void Candidates_changes_with_removePossibleDigit_Test()
        {
            var id = 0;
            var target = new Cell(id);
            ReadOnlyCollection<int> actual;
            target.RemoveCandidate(3, new SudokuLog());
            actual = target.Candidates;
            Assert.AreEqual(Consts.DimensionSquare - 1, actual.Count);
            for (var i = 1; i <= Consts.DimensionSquare; i++)
            {
                if (i != 3)
                {
                    Assert.IsTrue(actual.Contains(i));
                }
            }
        }

        [TestMethod]
        public void CandidateValue_Digit_is_0_when_BaseValue_was_set_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 0;
            int actual;
            target.Digit = 1;
            actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
            target.CandidateValue = 3;
            actual = target.Digit;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CandidateValue_fire_OnPropertyChanged_event_when_value_changes_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 3;
            int actual;
            var propertyChangeWasDone = false;
            target.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, e) =>
            {
                Assert.AreEqual("CandidateValue", e.PropertyName);
                Assert.AreEqual(sender, target);
                Assert.AreEqual(((Cell)sender).CandidateValue, expected);
                propertyChangeWasDone = true;
            });
            target.CandidateValue = expected;
            actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(propertyChangeWasDone);
        }

        [TestMethod]
        public void CandidateValue_is_0_when_Digit_set_Test()
        {
            var target = new Cell(0)
            {
                Digit = 1
            };
            Assert.AreEqual(0, target.CandidateValue);
        }

        /// <summary>
        ///A test for CheckLastDigit
        ///</summary>
        [TestMethod]
        [DeploymentItem("Sudoku.exe")]
        public void CheckLastDigit_returns_false_when_not_last_candidate_after_create_Test()
        {
            var target = new Cell(0);
            SudokuLog sudokuResult = null;
            var expected = false;
            var actual = target.CheckLastDigit(sudokuResult);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CheckLastDigit
        ///</summary>
        [TestMethod]
        [DeploymentItem("Sudoku.exe")]
        public void CheckLastDigit_returns_false_when_not_last_candidate_Test()
        {
            var target = new Cell(0);
            SudokuLog sudokuResult = null;
            var expected = false;
            target.CheckLastDigit(sudokuResult);
            for (var x = 0; x < Consts.DimensionSquare - 3; x++)
            {
                for (var y = x + 1; y < Consts.DimensionSquare - 2; y++)
                {
                    var newBaseValue = (1 << x) | (1 << y);
                    target.CandidateValue = newBaseValue;
                    var actual = target.CheckLastDigit(sudokuResult);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        ///A test for BaseValue
        ///</summary>
        [TestMethod]
        public void Constructor_BaseValue_is_Consts_BaseStart_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = Consts.BaseStart;
            var actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Constructor_HType_eq_HouseType_Cell_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = HouseType.Cell;
            var actual = target.HType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cell Constructor
        ///</summary>
        [TestMethod]
        public void Constructor_id_is_ID_Property_Test()
        {
            for (var id = 0; id < Consts.DimensionSquare; id++)
            {
                var target = new Cell(id);
                Assert.AreEqual(id, target.ID);
            }
        }

        /// <summary>
        ///A test for Digit
        ///</summary>
        [TestMethod]
        public void Digit_does_not_set_Digit_when_not_in_range_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 0;
            int actual;
            try
            {
                target.Digit = -1;
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Digit -1 is in Cell(0) [A1] 0 not possible", ex.Message);
            }
            finally
            {
                actual = target.Digit;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Digit_fire_OnPropertyChanged_event_when_Digit_changes_twice_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 3;
            int actual;
            var propertyChangeCandidateWasDone = false;
            var propertyChangeDigitWasDone = false;
            target.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, e) =>
            {
                if (e.PropertyName.Equals("Digit"))
                {
                    Assert.AreEqual(sender, target);
                    Assert.AreEqual(((Cell)sender).Digit, expected);
                    propertyChangeDigitWasDone = true;
                }
                else if (e.PropertyName.Equals("CandidateValue"))
                {
                    Assert.AreEqual(sender, target);
                    Assert.AreEqual(((Cell)sender).CandidateValue, 0);
                    propertyChangeCandidateWasDone = true;
                }
            });
            target.Digit = expected;
            actual = target.Digit;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.CandidateValue);
            Assert.IsTrue(propertyChangeCandidateWasDone);
            Assert.IsTrue(propertyChangeDigitWasDone);
        }

        /// <summary>
        ///A test for Digit
        ///</summary>
        [TestMethod]
        public void Digit_set_Digit_changes_BaseValue_to_0_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 0;
            int actual;
            target.Digit = 3;
            actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Digit_set_Digit_removes_Candidates_in_Houses_and_check_single_candidate_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var row = new Cell[2];
            var col = new Cell[2];
            var box = new Cell[2];
            for (var i = 0; i < 2; i++)
            {
                row[i] = new Cell(i);
                col[i] = new Cell(i + 10);
                box[i] = new Cell(i + 20);
            }
            target._fieldcontainters[0] = new House<Cell>(row, HouseType.Row, 1);
            target._fieldcontainters[1] = new House<Cell>(col, HouseType.Col, 1);
            target._fieldcontainters[2] = new House<Cell>(box, HouseType.Box, 1);

            row[0].CandidateValue = 3;
            target.SetDigit(1);

            Assert.AreEqual(2, row[0].Digit);
        }

        [TestMethod]
        public void Digit_set_Digit_removes_Candidates_in_Houses_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var row = new Cell[10];
            var col = new Cell[10];
            var box = new Cell[10];
            for (var i = 0; i < 10; i++)
            {
                row[i] = new Cell(i);
                col[i] = new Cell(i + 10);
                box[i] = new Cell(i + 20);
            }
            target._fieldcontainters[0] = new House<Cell>(row, HouseType.Row, 1);
            target._fieldcontainters[1] = new House<Cell>(col, HouseType.Col, 1);
            target._fieldcontainters[2] = new House<Cell>(box, HouseType.Box, 1);

            var expected = Consts.BaseStart;
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(expected, row[i].CandidateValue);
                Assert.AreEqual(expected, col[i].CandidateValue);
                Assert.AreEqual(expected, box[i].CandidateValue);
            }
            expected = Consts.BaseStart - 1;
            target.SetDigit(1);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(expected, row[i].CandidateValue);
                Assert.AreEqual(expected, col[i].CandidateValue);
                Assert.AreEqual(expected, box[i].CandidateValue);
            }
        }

        /// <summary>
        ///A test for Digit
        ///</summary>
        [TestMethod]
        public void Digit_set_Digit_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var expected = 1;
            int actual;
            target.Digit = expected;
            actual = target.Digit;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void Equal_is_false_when_compare_with_null_Test()
        {
            var id = 0;
            var target = new Cell(id);
            object obj = null;
            var expected = false;
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void Equal_is_true_when_ID_is_equal_Test()
        {
            var id = 0;
            var target = new Cell(id);
            object obj = new Cell(id);
            var expected = true;
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod]
        public void GetHashCode_is_always_unique_Test()
        {
            var cells = new Cell[Consts.CountCell];
            for (var id = 0; id < Consts.CountCell; id++)
            {
                cells[id] = new Cell(id);
            }

            for (var id = 0; id < Consts.CountCell; id++)
            {
                for (var id2 = 0; id2 < Consts.CountCell; id2++)
                {
                    if (id == id2)
                    {
                        Assert.AreEqual(cells[id].GetHashCode(), cells[id2].GetHashCode());
                    }
                    else
                    {
                        Assert.AreNotEqual(cells[id].GetHashCode(), cells[id2].GetHashCode());
                    }
                }
            }
        }

        /// <summary>
        ///A test for RemovePossibleDigit
        ///</summary>
        [TestMethod]
        public void RemoveCandidate_BaseValue_changes_when_candidate_removes_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var digit = 1;
            var sudokuResult = new SudokuLog();
            var expected = true;
            bool actual;
            actual = target.RemoveCandidate(digit, sudokuResult);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Must set digit when only one candidate is left.
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        [TestMethod]
        public void RemoveCandidate_checks_lastcandidate_Test()
        {
            var id = 0;
            var target = new Cell(id)
            {
                CandidateValue = 7
            };
            var sudokuResult = new SudokuLog();
            var expected = true;
            bool actual;
            actual = target.RemoveCandidate(1, sudokuResult);
            Assert.AreEqual(expected, actual);
            actual = target.RemoveCandidate(2, sudokuResult);
            Assert.AreEqual(expected, actual);
            expected = false;
            actual = target.RemoveCandidate(3, sudokuResult);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(3, target.Digit);
        }

        /// <summary>
        ///A test for SetDigit
        ///</summary>
        [TestMethod]
        public void SetDigit_changes_Digit_peoperty_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var digit = 1;
            SudokuLog actual;
            actual = target.SetDigit(digit);
            Assert.IsTrue(actual.Successful);
            Assert.AreEqual(digit, target.Digit);
        }

        /// <summary>
        ///A test for SetDigit
        ///</summary>
        [TestMethod]
        public void SetDigit_return_true_when_digit_was_set_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var digitFromOutside = 1;
            var sudokuResult = new SudokuLog();
            var expected = true;
            bool actual;
            actual = target.SetDigit(digitFromOutside, sudokuResult);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SetDigit
        ///</summary>
        [TestMethod]
        public void SetDigit_set_BaseValue_to_0_Test()
        {
            var id = 0;
            var target = new Cell(id);
            var digit = 1;
            SudokuLog actual;
            actual = target.SetDigit(digit);
            Assert.IsTrue(actual.Successful);
            Assert.AreEqual(0, target.CandidateValue);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest_Test()
        {
            var id = 0; // TODO: Initialize to an appropriate value
            var target = new Cell(id); // TODO: Initialize to an appropriate value
            var expected = "Cell(0) [A1] 0";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
