using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DE.Onnen.Sudoku
{
    [TestClass]
    public class IBoardTest
    {
        private IBoard<Cell> target;

        [TestInitialize]
        public void Initialize()
        {
            this.target = new Board();
            int digit = 1;
            int cell = 0;
            this.target.SetDigit(cell, digit);
        }

        /// <summary>
        /// Digit must be set.
        ///</summary>
        [TestMethod]
        public void SetDigit_Digit_in_Cell_must_be_set_Test()
        {
            Assert.AreEqual(1, this.target[0].Digit);
            Assert.AreEqual(0, this.target[0].CandidateValue);
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_col_Test()
        {
            int baseValue = (1 << Consts.DimensionSquare) - 1;
            int expected = baseValue - (1 << 0);
            for (int i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, this.target[i * 9].Digit);
                Assert.AreEqual(expected, this.target[i * 9].CandidateValue);
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_row_Test()
        {
            int baseValue = (1 << Consts.DimensionSquare) - 1;
            int expected = baseValue - (1 << 0);
            for (int i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, this.target[i].Digit);
                Assert.AreEqual(expected, this.target[i].CandidateValue);
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_box_Test()
        {
            int baseValue = (1 << Consts.DimensionSquare) - 1;
            int expected = baseValue - (1 << 0);
            int containerIdx = 0;
            for (int zr = 0; zr < Consts.Dimension; zr++)
            {
                for (int zc = 0; zc < Consts.Dimension; zc++)
                {
                    int b = (containerIdx * Consts.Dimension) + (zc + (zr * Consts.DimensionSquare)) + ((containerIdx / Consts.Dimension) * Consts.DimensionSquare * 2);
                    if (b == 0)
                    {
                        continue;
                    }
                    Assert.AreEqual(0, this.target[b].Digit);
                    Assert.AreEqual(expected, this.target[b].CandidateValue);
                }
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_peer_row_Test()
        {
            int baseValue = (1 << Consts.DimensionSquare) - 1;
            int expected = baseValue - (1 << 0);

            for (int i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, this.target[i].Digit);
                Assert.AreEqual(expected, this.target[i].CandidateValue);
            }
        }
    }
}