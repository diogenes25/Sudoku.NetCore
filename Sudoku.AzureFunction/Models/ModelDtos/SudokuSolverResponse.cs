using DE.Onnen.Sudoku.Serialization;

namespace Sudoku.AzureFunction.Models.ModelDtos
{
    /// <summary>
    /// Represents the response from the Sudoku solver.
    /// </summary>
    public record SudokuSolverResponse
    {
        /// <summary>
        /// Gets or sets the Sudoku board information.
        /// </summary>
        public SudokuDto? SudokuBoardInfo { get; set; }
    }
}
