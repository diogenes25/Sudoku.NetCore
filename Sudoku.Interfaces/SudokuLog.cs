using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
    public class SudokuLog
    {
        private bool successful = true;

        public SudokuEvent EventInfoInResult { get; set; }

        public bool Successful
        {
            get
            {
                bool result = this.successful;
                foreach (SudokuLog sr in this.ChildSudokuResult)
                {
                    result &= sr.Successful;
                }
                return result;
            }
            set { this.successful = value; }
        }

        public string ErrorMessage { get; set; }

        public SudokuLog ParentSudokuResult { get; set; }

        public List<SudokuLog> ChildSudokuResult { get; set; }

        public SudokuLog()
        {
            this.ChildSudokuResult = new List<SudokuLog>();
        }

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
    }
}