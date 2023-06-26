using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    ///This is a test class for CellTest and is intended
    ///to contain all CellTest Unit Tests
    ///</summary>
    [TestClass]
    public class CellTests
    {
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
            var expected = EHouseType.Cell;
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
            target._fieldcontainters[0] = new House(row, EHouseType.Row, 1);
            target._fieldcontainters[1] = new House(col, EHouseType.Col, 1);
            target._fieldcontainters[2] = new House(box, EHouseType.Box, 1);

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
            target._fieldcontainters[0] = new House(row, EHouseType.Row, 1);
            target._fieldcontainters[1] = new House(col, EHouseType.Col, 1);
            target._fieldcontainters[2] = new House(box, EHouseType.Box, 1);

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

        [TestMethod]
        public void GetUniqueID_Test()
        {
            var target = new Cell(0);
            var uniqueID = target.GetUniqueID();
            Assert.AreEqual(uniqueID, 65408, "Every Candidate is set");

            var bitArray = ConvertIntToBitArray(uniqueID);
            Assert.IsTrue(bitArray[7], "Bit 7 (represents candidate 1) must be set");
            Assert.IsTrue(bitArray[8], "Bit 8 (represents candidate 2) must be set");
            Assert.IsTrue(bitArray[9], "Bit 9 (represents candidate 3) must be set");
            Assert.IsTrue(bitArray[10], "Bit 10 (represents candidate 4) must be set");
            Assert.IsTrue(bitArray[11], "Bit 11 (represents candidate 5) must be set");
            Assert.IsTrue(bitArray[12], "Bit 12 (represents candidate 6) must be set");
            Assert.IsTrue(bitArray[13], "Bit 13 (represents candidate 7) must be set");
            Assert.IsTrue(bitArray[14], "Bit 14 (represents candidate 8) must be set");
            Assert.IsTrue(bitArray[15], "Bit 15 (represents candidate 9) must be set");
            Assert.IsFalse(bitArray[16], "Bit 16 should be 0 because there are no more candidates");

            var target2 = new Cell(1);
            var uniqueID2 = target2.GetUniqueID();
            var bitArray2 = ConvertIntToBitArray(uniqueID2);
            Assert.IsTrue(bitArray2[0], "CellID is 1");

            target2 = new Cell(6);
            uniqueID2 = target2.GetUniqueID();
            bitArray2 = ConvertIntToBitArray(uniqueID2);
            Assert.IsFalse(bitArray2[0], "CellID is 6 and bit nr 1 must not be set 011");
            Assert.IsTrue(bitArray2[1], "CellID is 6 and bit nr 2 must be set 011");
            Assert.IsTrue(bitArray2[2], "CellID is 6 and bit nr 3 must be set 011");
            Assert.IsFalse(bitArray2[3], "CellID is 6 and bit nr 4 must not be set 011");

            var targetDigit = new Cell(1)
            {
                Digit = 7,
            };
            var digitUniqueID = targetDigit.GetUniqueID();
            Assert.IsTrue(digitUniqueID < 0, "When call.digit is set, the uniqueId must be negative");
            var digitUniqueIDPositive = digitUniqueID * -1;
            var digitBitArray = ConvertIntToBitArray(digitUniqueIDPositive);
            Assert.IsTrue(digitBitArray[0], "CellID is 1");
            Assert.IsTrue(digitBitArray[7], "Bit 7 (first Bit for 7 (1110)) must be set");
            Assert.IsTrue(digitBitArray[8], "Bit 8 (second Bit for 7 (1110)) must be set");
            Assert.IsTrue(digitBitArray[9], "Bit 9 (third Bit for 7 (1110)) must be set");
            Assert.IsFalse(digitBitArray[10], "Bit 10 (fourth Bit for 7 (1110)) must not be set");
        }

        public static bool[] ConvertIntToBitArray(int value)
        {
            var bitArray = new bool[32];
            for (var i = 0; i < 32; i++)
            {
                bitArray[i] = (value & (1 << i)) > 0;
            }
            return bitArray;
        }

        [TestMethod]
        public void ClearTest()
        {
            var cell = new Cell(0)
            {
                Digit = 1
            };
            Assert.AreEqual(1, cell.Digit, "Digit was set to 1 and must be 1.");
            Assert.AreEqual(0, cell.CandidateValue, "When a Digit was set, no candidates are.");
            cell.Clear();
            Assert.AreEqual(0, cell.Digit, "Cell was cleared and Digit must set to 0");
            Assert.AreEqual(Consts.BASESTART, cell.CandidateValue, "Cell was cleared and every candidate must be available");
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            var cell = new Cell(0)
            {
                Digit = 1
            };
            var actual = cell.GetHashCode();
            Assert.IsTrue(actual != 0);
            var cell2 = new Cell(0)
            {
                Digit = 2
            };
            var actual2 = cell2.GetHashCode();
            Assert.AreNotEqual(actual, actual2);

            cell.SetDigit(0);
            var actualN = cell.GetHashCode();
            Assert.AreNotEqual(actual, actualN);

            cell = new Cell(0);
            cell.RemoveCandidate(1, new SudokuLog());
            actual = cell.GetHashCode();
            Assert.IsTrue(actual != 0);
            cell2 = new Cell(0);
            cell2.RemoveCandidate(2, new SudokuLog());
            actual2 = cell2.GetHashCode();
            Assert.AreNotEqual(actual, actual2);
        }

        [TestMethod]
        public void RemoveCandidateTest()
        {
            var target = new Cell(0);
            var log = new SudokuLog();
            target.RemoveCandidate(2, log);
            var uniqueID = target.GetUniqueID();

            var bitArray = ConvertIntToBitArray(uniqueID);
            Assert.IsTrue(bitArray[7], "Bit 7 (represents candidate 1) must be set");
            Assert.IsFalse(bitArray[8], "Bit 8 (represents candidate 2) must NOT be set because it is removed");
            Assert.IsTrue(bitArray[9], "Bit 9 (represents candidate 3) must be set");
            Assert.IsTrue(bitArray[10], "Bit 10 (represents candidate 4) must be set");
            Assert.IsTrue(bitArray[11], "Bit 11 (represents candidate 5) must be set");
            Assert.IsTrue(bitArray[12], "Bit 12 (represents candidate 6) must be set");
            Assert.IsTrue(bitArray[13], "Bit 13 (represents candidate 7) must be set");
            Assert.IsTrue(bitArray[14], "Bit 14 (represents candidate 8) must be set");
            Assert.IsTrue(bitArray[15], "Bit 15 (represents candidate 9) must be set");
            Assert.IsFalse(bitArray[16], "Bit 16 should be 0 because there are no more candidates");
        }
    }
}
