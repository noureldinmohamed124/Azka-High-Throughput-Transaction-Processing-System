using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Domain.Entities
{
    public class ReceiptSequence
    {
        public int Id { get; set; }
        public ReceiptPrefixEnum Prefix { get; set; }

        public DateOnly Date { get; set; }

        public int LastSequence { get; set; }
    }
}
