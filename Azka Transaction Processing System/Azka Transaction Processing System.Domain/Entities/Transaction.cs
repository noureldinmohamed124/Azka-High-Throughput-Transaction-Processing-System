using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Azka_Transaction_Processing_System.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public int PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public TransactionStatusEnum Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? SettledOn { get; set; }

        public Customer Customer { get; set; } = null!;

        public Branch Branch { get; set; } = null!;

        public PaymentMethod PaymentMethod { get; set; } = null!;
    }
}
