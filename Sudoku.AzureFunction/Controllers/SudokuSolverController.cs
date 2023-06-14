﻿using System.Net;
using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.Extensions;
using DE.Onnen.Sudoku.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Sudoku.AzureFunction.Controllers
{
    public class SudokuSolverController
    {
        private readonly Board _board;

        private readonly ILogger _logger;

        public SudokuSolverController(Board board, ILoggerFactory loggerFactory)
        {
            _board = board;
            _logger = loggerFactory.CreateLogger<SudokuSolverController>();
        }

        [Function("PostSolve")]
        public async Task<HttpResponseData> SolveAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Solve.");
            var sudokuTransfer = await req.ReadFromJsonAsync<SudokuDto>();

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
            var transfer = new SudokuDto
            {
                Cells = new List<int>(_board.CreateSimpleBoard()),
                Action = sudokuTransfer.Action,
            };
            await response.WriteAsJsonAsync(transfer);
            return response;
        }

        [Function("GetSolve")]
        public HttpResponseData StarterBoard([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            if (req.Query.AllKeys.Contains("board"))
            {
                _board.SetCellsFromString(req.Query["board"]);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            var transfer = new SudokuDto
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
            response.WriteAsJsonAsync(transfer);
            return response;
        }

        [Function("GetSolveAsHtml")]
        public async Task<HttpResponseData> SolveBoardReturnHtmlAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            if (req.Query.AllKeys.Contains("board"))
            {
                _board.SetCellsFromString(req.Query["board"]);
                // var sudokuLog = new SudokuLog();
                var sudokuLog = _board.StartSolve();
            }

            var sudokuTableHtml = _board.ToHtmlTable();
            var response = req.CreateResponse(HttpStatusCode.OK);
            var css = "<meta charset=\"utf-8\" />\r\n\t<title></title>\r\n\t<style>\r\n\t .sudokutbl {border: solid 2px black;color: black;}\r\n\t .sudokucell  {border: solid 2px green;background-color: grey;}\r\n\t .sudokucell_given {border: solid 2px green;background-color: green;}\r\n\t</style>";
            var html = $"<html><head>{css}</head><body>{sudokuTableHtml}</body></html>";
            await response.WriteStringAsync(html);
            return response;
        }
    }
}
