using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Common.Extensions
{
    public static class ReceiptPrefixExtensions
    {
        public static string ToCode(this TransactionTypeEnum prefix)
        {
            return prefix switch
            {
                TransactionTypeEnum.Payment => "PAY",
                TransactionTypeEnum.Refund => "REF",
                TransactionTypeEnum.Reversal => "REV",
                TransactionTypeEnum.Chargeback => "CHB",
                _ => throw new ValidationException("Unsupported transaction type.")
            };
        }

        public static TransactionTypeEnum FromCode(string code)
        {
            return code.Trim().ToUpperInvariant() switch
            {
                "PAY" => TransactionTypeEnum.Payment,
                "REF" => TransactionTypeEnum.Refund,
                "REV" => TransactionTypeEnum.Reversal,
                "CHB" => TransactionTypeEnum.Chargeback,
                _ => throw new ValidationException("Unsupported transaction type.")
            };
        }
    }
}
