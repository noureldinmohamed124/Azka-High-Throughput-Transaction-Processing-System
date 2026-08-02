using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Modules.PaymentMethods.GetPaymentMethods;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IReadOnlyList<PaymentMethodResponse>> GetAllPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new PaymentMethodResponse
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }
    }
}
