using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
    public class SudokuLog
    {
        #region Private Fields

        private bool _successful = true;

        #endregion Private Fields

        #region Public Constructors

        public SudokuLog()
        {
            this.ChildSudokuResult = new List<SudokuLog>();
        }

        #endregion Public Constructors

        #region Public Properties

        public List<SudokuLog> ChildSudokuResult { get; set; }

        public string ErrorMessage { get; set; }

        public SudokuEvent EventInfoInResult { get; set; }

        public SudokuLog ParentSudokuResult { get; set; }

        public bool Successful
        {
            get
            {
                bool result = this._successful;
                foreach (SudokuLog sr in this.ChildSudokuResult)
                {
                    result &= sr.Successful;
                }
                return result;
            }
            set { this._successful = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public SudokuLog CreateChildResult()
        {
            SudokuLog child = new SudokuLog
            {
                ParentSudokuResult = this
            };
            this.ChildSudokuResult.Add(child);
            return child;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Successful);
            if (this.EventInfoInResult != null)
            {
                sb.Append(", ");
                sb.Append(this.EventInfoInResult.ToString());
            }
            return sb.ToString();
        }

        #endregion Public Methods
    }
}
