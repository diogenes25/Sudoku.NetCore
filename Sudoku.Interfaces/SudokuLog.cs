using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
    public class SudokuLog
    {
        private bool _successful = true;

        public List<SudokuLog> ChildSudokuResult { get; set; } = new List<SudokuLog>();

        public string ErrorMessage { get; set; }

        public SudokuEvent EventInfoInResult { get; set; }

        public SudokuLog ParentSudokuResult { get; set; }

        public bool Successful
        {
            get
            {
                var result = _successful;
                foreach (var sr in ChildSudokuResult)
                {
                    result &= sr.Successful;
                    if(!result) return false; // if result is already false, there is no need to continue to check the childResults.
                }
                return result;
            }
            set => _successful = value;
        }

        public SudokuLog CreateChildResult()
        {
            var child = new SudokuLog
            {
                ParentSudokuResult = this
            };
            ChildSudokuResult.Add(child);
            return child;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Successful);
            if (EventInfoInResult != null)
            {
                sb.Append(", ");
                sb.Append(EventInfoInResult.ToString());
            }
            return sb.ToString();
        }
    }
}
