using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Domain.Enums
{
    public enum TransactionStatusEnum
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Settled = 4,
        Cancelled = 5
    }
}
