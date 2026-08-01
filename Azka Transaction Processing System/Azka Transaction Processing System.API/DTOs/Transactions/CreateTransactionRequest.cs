using Azka_Transaction_Processing_System.Domain.Enums;

namespace Azka_Transaction_Processing_System.API.DTOs.Transactions
{
    public class CreateTransactionRequest
    {
        // Payment - Refund - Reversal - Chargeback
        public string TransactionType { get; init; } = default!;
        public int BranchId { get; init; }
        public int PaymentMethodId { get; init; }
        public decimal Amount { get; init; }
        public string TransactionStatus { get; init; } = string.Empty;

        public DateTime? SettledOn { get; set; }
        public TransactionStatusEnum Status { get; set; }
    }
}

