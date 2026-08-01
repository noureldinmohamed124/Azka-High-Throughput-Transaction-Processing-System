using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Repositories
{
    public class PaymentMethodRepo : GenericRepo<PaymentMethod>, IPaymentMethodRepo
    {
        public PaymentMethodRepo(TPSDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentMethod?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
