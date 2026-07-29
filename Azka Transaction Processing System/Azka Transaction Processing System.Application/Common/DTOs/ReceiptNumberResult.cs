using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Common.DTOs
{
    public class ReceiptNumberResult
    {
        public string ReceiptNumber { get; init; } = string.Empty;
        public int Sequence { get; init; }
        public DateOnly Date { get; init; } 
    }
}
