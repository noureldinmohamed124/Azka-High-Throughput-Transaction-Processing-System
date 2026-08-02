using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.PaymentMethods.GetPaymentMethods
{
    public class GetPaymentMethodsUseCase
    {
        private readonly IPaymentMethodRepo _paymentMethodRepo;

        public GetPaymentMethodsUseCase(IPaymentMethodRepo paymentMethodRepo)
        {
            _paymentMethodRepo = paymentMethodRepo;
        }

        public async Task<IReadOnlyList<PaymentMethodResponse>> ExecuteAsync()
        {
            return await _paymentMethodRepo.GetAllPaymentMethodsAsync();
        }
    }
}
