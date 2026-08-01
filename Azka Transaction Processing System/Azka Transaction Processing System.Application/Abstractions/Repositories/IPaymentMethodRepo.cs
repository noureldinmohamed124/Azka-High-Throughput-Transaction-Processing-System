using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface IPaymentMethodRepo : IGenericRepo<PaymentMethod>
    {
        Task<bool> ExistsAsync(int id);

        Task<PaymentMethod?> GetByNameAsync(string name);
    }
}
