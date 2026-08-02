using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface ITransactionRepo : IGenericRepo<Transaction>
    {
        Task<Transaction?> GetTransactionDetailsByReceiptAsync(string receiptNumber);

        Task<IReadOnlyList<SearchTransactionSummaryResponse>> SearchAsync(int? customerId, DateOnly? date);

        Task<DailyTransactionSummaryResponse> GetDailySummaryAsync(DateOnly date);

    }
}
