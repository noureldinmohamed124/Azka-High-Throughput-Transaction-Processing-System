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
        public static string ToCode(this ReceiptPrefixEnum prefix)
        {
            return prefix switch
            {
                ReceiptPrefixEnum.Payment => "PAY",
                ReceiptPrefixEnum.Refund => "REF",
                ReceiptPrefixEnum.Reversal => "REV",
                ReceiptPrefixEnum.Chargeback => "CHB",
                _ => throw new ValidationException("Unsupported transaction type.")
            };
        }

        public static ReceiptPrefixEnum FromCode(string code)
        {
            return code.Trim().ToUpperInvariant() switch
            {
                "PAY" => ReceiptPrefixEnum.Payment,
                "REF" => ReceiptPrefixEnum.Refund,
                "REV" => ReceiptPrefixEnum.Reversal,
                "CHB" => ReceiptPrefixEnum.Chargeback,
                _ => throw new ValidationException("Unsupported transaction type.")
            };
        }
    }
}
