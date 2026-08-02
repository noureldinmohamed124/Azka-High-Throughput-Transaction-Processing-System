using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary
{
    public class DailyTransactionSummaryUseCase
    {
        private readonly ITransactionRepo _transactionRepo;

        public DailyTransactionSummaryUseCase(ITransactionRepo transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<DailyTransactionSummaryResponse> ExecuteAsync(DailyTransactionSummaryQuery query)
        {
            return await _transactionRepo.GetDailySummaryAsync(query.Date);
        }
    }
}
