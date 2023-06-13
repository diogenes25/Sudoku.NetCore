using DE.Onnen.Sudoku.Serialization;

namespace Sudoku.AzureFunction.Models.ModelDtos
{
    public record SudokuSolverResponse
    {
        public SudokuDto? SudokuBoardInfo { get; set; }
    }
}
