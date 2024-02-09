using System.Net;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Sudoku.AzureFunction.Controllers
{
    public class SudokuSolverController(Board board, ILoggerFactory loggerFactory)
    {
        private readonly Board _board = board;

        private readonly ILogger _logger = loggerFactory.CreateLogger<SudokuSolverController>();

        [Function("Solve")]
        public async Task<HttpResponseData> SolveAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Solve.");
            var sudokuTransfer = await req.ReadFromJsonAsync<SudokuDto>();

            if (sudokuTransfer != null)
            {
                if (sudokuTransfer.Cells != null)
                {
                    _board.FillBoardWithUniqueCellIDs(sudokuTransfer.Cells);
                }

                if (sudokuTransfer.Action != null)
                {
                    foreach (var action in sudokuTransfer.Action)
                    {
                        _board.SetDigit(action.CellId, action.Digit);
                    }
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            var transfer = new SudokuDto
            {
                Cells = new List<int>(_board.CreateSimpleBoard()),
                Action = sudokuTransfer?.Action,
            };
            await response.WriteAsJsonAsync(transfer);
            return response;
        }

        [Function("SolveTest")]
        public async Task<HttpResponseData> StarterBoardAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            if (req.Query.AllKeys.Contains("board"))
            {
                _board.SetCellsFromString(req.Query["board"]);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            var transfer = new SudokuDto
            {
                Cells = new List<int>(_board.CreateSimpleBoard()),
                Action =
                 [
                     new DigitAction
                     {
                         CellId = 1,
                         Digit = 2
                     }
                 ],
            };
            await response.WriteAsJsonAsync(transfer).ConfigureAwait(false);
            return response;
        }

        [Function("SolveHtml")]
        public async Task<HttpResponseData> SolveBoardReturnHtmlAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            if (req.Query.AllKeys.Contains("board"))
            {
                var sudokuStr = req.Query["board"];
                _board.SetCellsFromString(sudokuStr);
                var countOf0 = sudokuStr.ToCharArray().Count(c => c == '0'); // A '0' represends a cell without a digit
                if (countOf0 < 76) // If more than 4 numbers (76 '0' left) are set, the use of the solution algorithm only makes sense.
                {
                    _board.StartSolve();
                }
            }

            var sudokuTableHtml = _board.ToHtmlTableWithLinks();
            var response = req.CreateResponse(HttpStatusCode.OK);
            var css = "<meta charset=\"utf-8\" />\r\n\t<title></title>\r\n\t<style>\r\n\t .sudokutbl {border: solid 2px black;color: black;}\r\n\t .sudokucell  {border: solid 2px green;background-color: grey;}\r\n\t .sudokucell_given {border: solid 2px green;background-color: green;}\r\n\t</style>";
            var html = $"<html><head>{css}</head><body>{sudokuTableHtml}</body></html>";
            await response.WriteStringAsync(html);
            return response;
        }
    }
}
