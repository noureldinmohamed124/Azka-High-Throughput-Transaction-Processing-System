using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
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
    public class TransactionRepo : GenericRepo<Transaction>, ITransactionRepo
    {
        public TransactionRepo(TPSDbContext context) : base(context)
        {
        }

        public async Task<Transaction?> GetTransactionDetailsByReceiptAsync(string receiptNumber)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Branch)
                .Include(x => x.PaymentMethod)
                .FirstOrDefaultAsync(x => x.ReceiptNumber == receiptNumber);
        }

        public Task<bool> ReceiptExistsAsync(string receiptNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<Transaction>> SearchAsync(int? customerId, DateOnly? date, string? receiptNumber)
        {
            throw new NotImplementedException();
        }
    }
}
