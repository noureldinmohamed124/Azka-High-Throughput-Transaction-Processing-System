using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction
{
    public sealed class CreateTransactionCommand
    {
        public ReceiptPrefixEnum TransactionType { get; init; }
        public int CustomerId { get; set; }
        public int BranchId { get; init; }
        public int PaymentMethodId { get; init; }
        public decimal Amount { get; init; }
        public TransactionStatusEnum TransactionStatus { get; init; }
    }
}
