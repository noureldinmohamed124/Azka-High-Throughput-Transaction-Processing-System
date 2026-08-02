using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary
{
    public class DailyTransactionSummaryResponse
    {
        public DateOnly Date { get; init; }

        public int TotalTransactions { get; init; }

        public decimal TotalAmount { get; init; }

        public decimal AverageAmount { get; init; }

        public decimal LargestTransaction { get; init; }

        public decimal SmallestTransaction { get; init; }

        public IReadOnlyList<TransactionStatusSummary> Statuses { get; init; } = new List<TransactionStatusSummary>();
    }

    public class TransactionStatusSummary
    {
        public TransactionStatusEnum Status { get; init; }
        public int Count { get; init; }
        public decimal TotalAmount { get; init; }
    }
}
