using Azka_Transaction_Processing_System.API.Commons;
using Azka_Transaction_Processing_System.Application.Modules.PaymentMethods.GetPaymentMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Azka_Transaction_Processing_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : BaseApiController
    {
        private readonly GetPaymentMethodsUseCase _useCase;
        public PaymentMethodsController(GetPaymentMethodsUseCase useCase)
        {
            _useCase = useCase;
        }


        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _useCase.ExecuteAsync();
            return OkResponse(result);
        }
    }
}
