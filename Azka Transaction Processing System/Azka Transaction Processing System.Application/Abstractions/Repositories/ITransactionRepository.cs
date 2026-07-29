using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface ITransactionRepository : IGenericRepo<Transaction>
    {
        Task<Transaction?> GetByReceiptAsync(string receiptNumber);

        Task<List<Transaction>> SearchAsync(int? customerId, DateOnly? date, string? receiptNumber);

        Task<bool> ReceiptExistsAsync(string receiptNumber);
    }
}
