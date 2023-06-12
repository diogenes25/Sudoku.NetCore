using System;
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
        #region Public Methods

        /// <summary>
        ///A test for Candidates
        ///</summary>
        [TestMethod]
        public void Candidates_are_9_Test()
        {
            var actual = new Cell(0).Candidates;
            Assert.AreEqual(Consts.DIMENSIONSQUARE, actual.Count);
            for (var i = 0; i < Consts.DIMENSIONSQUARE; i++)
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
            var target = new Cell(0);
            target.RemoveCandidate(3, new SudokuLog());
            var actual = target.Candidates;
            Assert.AreEqual(Consts.DIMENSIONSQUARE - 1, actual.Count);
            for (var i = 1; i <= Consts.DIMENSIONSQUARE; i++)
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
            var target = new Cell(0)
            {
                Digit = 1,
            };
            var expected = 0;
            var actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
            target.CandidateValue = 3;
            actual = target.Digit;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CandidateValue_fire_OnPropertyChanged_event_when_value_changes_Test()
        {
            var target = new Cell(0);
            var expected = 3;
            var propertyChangeWasDone = false;
            target.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, e) =>
            {
                Assert.AreEqual("CandidateValue", e.PropertyName);
                Assert.AreEqual(sender, target);
                Assert.AreEqual(((Cell)sender).CandidateValue, expected);
                propertyChangeWasDone = true;
            });
            target.CandidateValue = expected;
            var actual = target.CandidateValue;
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
        /// A test for CheckLastDigit
        /// </summary>
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
        /// A test for CheckLastDigit
        /// </summary>
        [TestMethod]
        [DeploymentItem("Sudoku.exe")]
        public void CheckLastDigit_returns_false_when_not_last_candidate_Test()
        {
            var target = new Cell(0);
            SudokuLog sudokuResult = null;
            var expected = false;
            target.CheckLastDigit(sudokuResult);
            for (var x = 0; x < Consts.DIMENSIONSQUARE - 3; x++)
            {
                for (var y = x + 1; y < Consts.DIMENSIONSQUARE - 2; y++)
                {
                    var newBaseValue = (1 << x) | (1 << y);
                    target.CandidateValue = newBaseValue;
                    var actual = target.CheckLastDigit(sudokuResult);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        /// A test for BaseValue
        /// </summary>
        [TestMethod]
        public void Constructor_BaseValue_is_Consts_BaseStart_Test()
        {
            var target = new Cell(0);
            var expected = Consts.BASESTART;
            var actual = target.CandidateValue;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Constructor_HType_eq_HouseType_Cell_Test()
        {
            var target = new Cell(0);
            var expected = HouseType.Cell;
            var actual = target.HType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Cell Constructor
        /// </summary>
        [TestMethod]
        public void Constructor_id_is_ID_Property_Test()
        {
            for (var id = 0; id < Consts.DIMENSIONSQUARE; id++)
            {
                var target = new Cell(id);
                Assert.AreEqual(id, target.ID);
            }
        }

        /// <summary>
        /// A test for Digit
        /// </summary>
        [TestMethod]
        public void Digit_does_not_set_Digit_when_not_in_range_Test()
        {
            var target = new Cell(0);
            var expected = 0;
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
                Assert.AreEqual(expected, target.Digit);
            }
        }

        [TestMethod]
        public void Digit_fire_OnPropertyChanged_event_when_Digit_changes_twice_Test()
        {
            var target = new Cell(0);
            var expected = 3;
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
            Assert.AreEqual(expected, target.Digit);
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
            var target = new Cell(0)
            {
                Digit = 3
            };
            Assert.AreEqual(0, target.CandidateValue, "When a Digit ist set, no Candidates should be there");
        }

        [TestMethod]
        public void Digit_set_Digit_removes_Candidates_in_Houses_and_check_single_candidate_Test()
        {
            var target = new Cell(0);
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
            var target = new Cell(0);
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

            var expected = Consts.BASESTART;
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(expected, row[i].CandidateValue);
                Assert.AreEqual(expected, col[i].CandidateValue);
                Assert.AreEqual(expected, box[i].CandidateValue);
            }
            expected = Consts.BASESTART - 1;
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
            var target = new Cell(0)
            {
                Digit = 1
            };
            Assert.AreEqual(1, target.Digit, "Digit must set to 1");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void Equal_is_false_when_compare_with_null_Test()
        {
            var target = new Cell(0);
            Assert.IsFalse(target.Equals(null));
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void Equal_is_true_when_ID_is_equal_Test()
        {
            var target = new Cell(0);
            object obj = new Cell(0);
            Assert.IsTrue(target.Equals(obj));
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod]
        public void GetHashCode_is_always_unique_Test()
        {
            var cells = new Cell[Consts.COUNTCELL];
            for (var id = 0; id < Consts.COUNTCELL; id++)
            {
                cells[id] = new Cell(id);
            }

            for (var id = 0; id < Consts.COUNTCELL; id++)
            {
                for (var id2 = 0; id2 < Consts.COUNTCELL; id2++)
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
            var target = new Cell(0);
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
            var target = new Cell(0);
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
            var target = new Cell(0);
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
            var target = new Cell(0);
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
            var target = new Cell(0); // TODO: Initialize to an appropriate value
            var expected = "Cell(0) [A1] 0";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        #endregion Public Methods
    }
}
