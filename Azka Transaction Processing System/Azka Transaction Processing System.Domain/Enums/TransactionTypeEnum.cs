using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Domain.Enums
{
    public enum TransactionTypeEnum
    {
        Payment = 1,
        Refund = 2,
        Reversal = 3,
        Chargeback = 4
    }
}
