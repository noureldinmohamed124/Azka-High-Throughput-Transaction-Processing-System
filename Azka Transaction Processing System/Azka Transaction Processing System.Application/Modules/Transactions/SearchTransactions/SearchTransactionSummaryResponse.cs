using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions
{
    public class SearchTransactionSummaryResponse
    {
        public string ReceiptNumber { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public TransactionStatusEnum Status { get; init; }
        public DateTime CreatedOn { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string BranchName { get; init; } = string.Empty;
        public string PaymentMethodName { get; init; } = string.Empty;
    }
}
