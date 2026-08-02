using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.CancelTransaction
{
    public class CancelTransactionResponse
    {
        public string ReceiptNumber { get; init; } = string.Empty;
        public TransactionStatusEnum Status { get; init; }
        public DateTime CancelledOn { get; init; }
    }
}
