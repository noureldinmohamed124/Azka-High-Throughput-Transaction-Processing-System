using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction
{
    public sealed class CreateTransactionResponse
    {
        public int TransactionId { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedOn { get; init; }
    }
}
