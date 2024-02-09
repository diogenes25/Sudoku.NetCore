using System.Collections.Generic;
using System.Text;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Represents a log entry for a Sudoku operation.
    /// </summary>
    public class SudokuLog
    {
        private bool _successful = true;

        /// <summary>
        /// Gets or sets the list of child Sudoku log entries.
        /// </summary>
        public List<SudokuLog> ChildSudokuResult { get; set; } = [];

        /// <summary>
        /// Gets or sets the error message associated with the Sudoku operation.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the event information associated with the Sudoku operation.
        /// </summary>
        public SudokuEvent EventInfoInResult { get; set; }

        /// <summary>
        /// Gets or sets the parent Sudoku log entry.
        /// </summary>
        public SudokuLog ParentSudokuResult { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Sudoku operation was successful.
        /// </summary>
        public bool Successful
        {
            get
            {
                var result = _successful;
                foreach (var sr in ChildSudokuResult)
                {
                    result &= sr.Successful;
                    if (!result)
                    {
                        return false; // if result is already false, than there is no need to continue to check the childResults.
                    }
                }
                return result;
            }
            set => _successful = value;
        }

        /// <summary>
        /// Creates a child Sudoku log entry and adds it to the list of child entries.
        /// </summary>
        /// <returns>The created child Sudoku log entry.</returns>
        public SudokuLog CreateChildResult()
        {
            var child = new SudokuLog
            {
                ParentSudokuResult = this
            };
            ChildSudokuResult.Add(child);
            return child;
        }

        /// <summary>
        /// Returns a string representation of the Sudoku log entry.
        /// </summary>
        /// <returns>A string representation of the Sudoku log entry.</returns>
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
