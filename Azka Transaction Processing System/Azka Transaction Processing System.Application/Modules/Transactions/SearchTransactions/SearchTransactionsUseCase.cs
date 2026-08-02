using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions
{
    public class SearchTransactionsUseCase
    {
        private readonly ITransactionRepo _transactionRepo;

        public SearchTransactionsUseCase(ITransactionRepo transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<IReadOnlyList<SearchTransactionSummaryResponse>> ExecuteAsync(SearchTransactionsQuery query)
        {
            return await _transactionRepo.SearchAsync(query.CustomerId, query.Date);
        }
    }
}
