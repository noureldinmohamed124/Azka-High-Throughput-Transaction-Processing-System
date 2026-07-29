namespace Azka_Transaction_Processing_System.API.DTOs.Transactions
{
    public class CreateTransactionRequest
    {
        public string TransactionType { get; init; } = default!;
        public int CustomerId { get; init; }
        public int BranchId { get; init; }
        public int PaymentMethodId { get; init; }
        public decimal Amount { get; init; }
        public string TransactionStatus { get; init; } = string.Empty;

    }
}

