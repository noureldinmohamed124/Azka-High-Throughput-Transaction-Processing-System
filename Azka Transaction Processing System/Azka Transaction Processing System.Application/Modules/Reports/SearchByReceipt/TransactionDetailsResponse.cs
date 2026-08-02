using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Reports.SearchByReceipt
{
    public class TransactionDetailsResponse
    {
        public string ReceiptNumber { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public TransactionStatusEnum Status { get; init; }
        public DateTime CreatedOn { get; init; }
        public DateTime? SettledOn { get; init; }
        public CustomerDto Customer { get; init; } = null!;
        public BranchDto Branch { get; init; } = null!;
        public PaymentMethodDto PaymentMethod { get; init; } = null!;
    }

    public class CustomerDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
    public class BranchDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public class PaymentMethodDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
