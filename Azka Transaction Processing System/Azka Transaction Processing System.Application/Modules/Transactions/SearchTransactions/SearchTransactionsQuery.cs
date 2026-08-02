using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions
{
    public class SearchTransactionsQuery
    {
        public int? CustomerId { get; init; }
        public DateOnly? Date { get; init; }
    }
}
