using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DE.Onnen.Sudoku
{
    [TestClass]
    public class IBoardTests
    {
        private IBoard<Cell> _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new Board();
            _target.SetDigit(cellID: 0, digitToSet: 1);
        }

        /// <summary>
        /// Digit must be set.
        ///</summary>
        [TestMethod]
        public void SetDigit_Digit_in_Cell_must_be_set_Test()
        {
            Assert.AreEqual(1, _target[0].Digit);
            Assert.AreEqual(0, _target[0].CandidateValue);
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_box_Test()
        {
            var baseValue = (1 << Consts.DIMENSIONSQUARE) - 1;
            var expected = baseValue - (1 << 0);
            var containerIdx = 0;
            for (var zr = 0; zr < Consts.DIMENSION; zr++)
            {
                for (var zc = 0; zc < Consts.DIMENSION; zc++)
                {
                    var b = (containerIdx * Consts.DIMENSION) + (zc + (zr * Consts.DIMENSIONSQUARE)) + ((containerIdx / Consts.DIMENSION) * Consts.DIMENSIONSQUARE * 2);
                    if (b == 0)
                    {
                        continue;
                    }
                    Assert.AreEqual(0, _target[b].Digit);
                    Assert.AreEqual(expected, _target[b].CandidateValue);
                }
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_col_Test()
        {
            var baseValue = (1 << Consts.DIMENSIONSQUARE) - 1;
            var expected = baseValue - (1 << 0);
            for (var i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, _target[i * 9].Digit);
                Assert.AreEqual(expected, _target[i * 9].CandidateValue);
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_peer_row_Test()
        {
            var baseValue = (1 << Consts.DIMENSIONSQUARE) - 1;
            var expected = baseValue - (1 << 0);

            for (var i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, _target[i].Digit);
                Assert.AreEqual(expected, _target[i].CandidateValue);
            }
        }

        [TestMethod]
        public void SetDigit_Digit_removed_as_candidate_in_row_Test()
        {
            var baseValue = (1 << Consts.DIMENSIONSQUARE) - 1;
            var expected = baseValue - (1 << 0);
            for (var i = 1; i < 9; i++)
            {
                Assert.AreEqual(0, _target[i].Digit);
                Assert.AreEqual(expected, _target[i].CandidateValue);
            }
        }
    }
}
