using Azka_Transaction_Processing_System.API.Commons;
using Azka_Transaction_Processing_System.API.DTOs.Transactions;
using Azka_Transaction_Processing_System.Application.Modules.Reports.SearchByReceipt;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
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
        private readonly SearchTransactionByReceiptNumberUseCase _searchTransactionByReceiptNumberUseCase;

        public TransactionsController(CreateTransactionUseCase createTransactionUseCase, SearchTransactionByReceiptNumberUseCase searchTransactionByReceiptNumberUseCase)
        {
            _createTransactionUseCase = createTransactionUseCase;
            _searchTransactionByReceiptNumberUseCase = searchTransactionByReceiptNumberUseCase;
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
            var query = new SearchTransactionByReceiptNumberQuery
            {
                RecieptNumber = receiptNumber
            };
            var result = await _searchTransactionByReceiptNumberUseCase.ExecuteAsync(query);
            return OkResponse(result);
        }


    }
}
