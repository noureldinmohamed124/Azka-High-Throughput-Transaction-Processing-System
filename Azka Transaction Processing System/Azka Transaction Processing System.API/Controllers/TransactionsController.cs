using Azka_Transaction_Processing_System.API.Commons;
using Azka_Transaction_Processing_System.API.DTOs.Transactions;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.CancelTransaction;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.GetTransactionByReceipt;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Azka_Transaction_Processing_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : BaseApiController
    {
        private readonly CreateTransactionUseCase _createTransactionUseCase;
        private readonly GetTransactionByReceiptUseCase _searchTransactionByReceiptNumberUseCase;
        private readonly SearchTransactionsUseCase _searchTransactionsUseCase;
        private readonly DailyTransactionSummaryUseCase _dailyTransactionSummaryUseCase;
        private readonly CancelTransactionUseCase _cancelTransactionUseCase;

        public TransactionsController(CreateTransactionUseCase createTransactionUseCase, GetTransactionByReceiptUseCase searchTransactionByReceiptNumberUseCase, SearchTransactionsUseCase searchTransactionsUseCase, DailyTransactionSummaryUseCase dailyTransactionSummaryUseCase, CancelTransactionUseCase cancelTransactionUseCase)
        {
            _createTransactionUseCase = createTransactionUseCase;
            _searchTransactionByReceiptNumberUseCase = searchTransactionByReceiptNumberUseCase;
            _searchTransactionsUseCase = searchTransactionsUseCase;
            _dailyTransactionSummaryUseCase = dailyTransactionSummaryUseCase;
            _cancelTransactionUseCase = cancelTransactionUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            var command = new CreateTransactionCommand
            {
                Amount = request.Amount,
                BranchId = request.BranchId,
                PaymentMethodId = request.PaymentMethodId,
                TransactionStatus = request.TransactionStatus,
                TransactionType = request.TransactionType,
                SettledOn = request.SettledOn
            };
            var result = await _createTransactionUseCase.ExecuteAsync(command);
            return OkResponse(result);
        }


        [AllowAnonymous]
        [HttpGet("receipt/{receiptNumber}")]
        public async Task<IActionResult> GetTransactionByReceiptNumber(string receiptNumber)
        {
            var query = new GetTransactionByReceiptQuery
            {
                RecieptNumber = receiptNumber
            };
            var result = await _searchTransactionByReceiptNumberUseCase.ExecuteAsync(query);
            return OkResponse(result);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SearchTransactions([FromQuery] SearchTransactionsRequest request)
        {
            var query = new SearchTransactionsQuery
            {
                CustomerId = request.CustomerId,
                Date = request.Date
            };

            var result = await _searchTransactionsUseCase.ExecuteAsync(query);

            return OkResponse(result);

        }


        [AllowAnonymous]
        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateOnly date)
        {
            var query = new DailyTransactionSummaryQuery 
            {
                Date = date 
            };
            var response = await _dailyTransactionSummaryUseCase.ExecuteAsync(query);
            return OkResponse(response);
        }


        [AllowAnonymous]
        [HttpPost("{receiptNumber}/cancel")]
        public async Task<IActionResult> CancelTransaction(string receiptNumber)
        {
            var response = await _cancelTransactionUseCase.ExecuteAsync(receiptNumber);
            return OkResponse(response);
        }

    }
}
