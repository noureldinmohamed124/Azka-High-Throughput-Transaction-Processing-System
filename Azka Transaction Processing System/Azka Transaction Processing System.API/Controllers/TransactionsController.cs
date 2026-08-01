using Azka_Transaction_Processing_System.API.Commons;
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

        public TransactionsController(CreateTransactionUseCase createTransactionUseCase)
        {
            _createTransactionUseCase = createTransactionUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> CreateTransaction()
        {
            var cc = await _createTransactionUseCase.ExecuteAsync();
            return OkResponse("");
        }
    }
}
