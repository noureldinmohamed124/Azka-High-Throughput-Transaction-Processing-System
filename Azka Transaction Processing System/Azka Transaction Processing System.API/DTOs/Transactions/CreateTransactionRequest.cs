using Azka_Transaction_Processing_System.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Azka_Transaction_Processing_System.API.DTOs.Transactions
{
    public class CreateTransactionRequest
    {
        // Payment - Refund - Reversal - Chargeback
        public TransactionTypeEnum TransactionType { get; init; } = default!;
        public int BranchId { get; init; }
        public int PaymentMethodId { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount Can't be less than 1")]
        public decimal Amount { get; init; }
        public DateTime? SettledOn { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
    }
}

