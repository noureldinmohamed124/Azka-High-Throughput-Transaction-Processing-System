using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.GetTransactionByReceiptNumber
{
    public class GetTransactionByReceiptNumberQuery
    {
        public string RecieptNumber { get; init; } = string.Empty;
    }
}
