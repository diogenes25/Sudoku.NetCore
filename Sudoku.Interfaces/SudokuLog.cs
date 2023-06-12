using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
    public class SudokuLog
    {
        #region Private Fields

        private bool _successful = true;

        #endregion Private Fields

        #region Public Properties

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
                }
                return result;
            }
            set => _successful = value;
        }

        #endregion Public Properties

        #region Public Methods

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

        #endregion Public Methods
    }
}
