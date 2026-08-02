namespace Azka_Transaction_Processing_System.API.DTOs.Transactions
{
    public class SearchTransactionsRequest
    {
        public int? CustomerId { get; init; }

        public DateOnly? Date { get; init; }
    }
}
