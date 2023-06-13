using System.Net;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Sudoku.AzureFunction
{
    public class SudokuSolver
    {
        private readonly Board _board;

        private readonly ILogger _logger;

        public SudokuSolver(Board board, ILoggerFactory loggerFactory)
        {
            _board = board;
            _logger = loggerFactory.CreateLogger<SudokuSolver>();
        }

        [Function("PostSolve")]
        public async Task<HttpResponseData> SolveAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var sudokuTransfer = await req.ReadFromJsonAsync<SudokuTransfer>();

            if (sudokuTransfer is null || sudokuTransfer.Cells?.Count < 1)
            {
                throw new ArgumentException("No Cells");
            }

            _board.FillBoardWithUniqueCellIDs(sudokuTransfer.Cells);

            foreach (var action in sudokuTransfer.Action)
            {
                _board.SetDigit(action.CellId, action.Digit);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            var transfer = new SudokuTransfer
            {
                Cells = new List<int>(_board.CreateSimpleBoard()),
                Action = sudokuTransfer.Action,
            };
            await response.WriteAsJsonAsync<SudokuTransfer>(transfer);
            return response;
        }

        [Function("GetSolve")]
        public HttpResponseData StarterBoard([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            if (req.Query.AllKeys.Contains("board"))
            {
                _board.SetCellsFromString(req.Query["board"]);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            var transfer = new SudokuTransfer
            {
                Cells = new List<int>(_board.CreateSimpleBoard()),
                Action = new List<DigitAction>
                 {
                     new DigitAction
                     {
                         CellId = 1,
                         Digit = 2
                     }
                 },
            };
            _ = response.WriteAsJsonAsync<SudokuTransfer>(transfer);
            return response;
        }
    }
}
