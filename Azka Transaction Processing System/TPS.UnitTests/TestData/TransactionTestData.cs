using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;

namespace TPS.UnitTests.TestData
{
    public static class TransactionTestData
    {
        public static CreateTransactionCommand CreateValidCommand()
        {
            return new CreateTransactionCommand
            {
                CustomerId = 1,
                BranchId = 1,
                PaymentMethodId = 1,
                Amount = 500.00m,
                TransactionType = TransactionTypeEnum.Payment,
                TransactionStatus = TransactionStatusEnum.Pending,
                SettledOn = DateTime.UtcNow
            };
        }

        public static Transaction CreateValidTransaction(int id = 1, string receiptNumber = "PAY-20260802-1-000001")
        {
            return new Transaction
            {
                Id = id,
                ReceiptNumber = receiptNumber,
                CustomerId = 1,
                BranchId = 1,
                PaymentMethodId = 1,
                Amount = 500.00m,
                Status = TransactionStatusEnum.Pending,
                CreatedOn = DateTime.UtcNow,
                Customer = CustomerTestData.CreateValidCustomer(),
                Branch = BranchTestData.CreateValidBranch(),
                PaymentMethod = PaymentMethodTestData.CreateValidPaymentMethod()
            };
        }

        public static SearchTransactionSummaryResponse CreateSearchSummaryResponse()
        {
            return new SearchTransactionSummaryResponse
            {
                ReceiptNumber = "PAY-20260802-1-000001",
                Amount = 500.00m,
                Status = TransactionStatusEnum.Pending,
                CreatedOn = DateTime.UtcNow,
                CustomerName = "John Doe",
                BranchName = "Main Branch",
                PaymentMethodName = "Visa Card"
            };
        }

        public static DailyTransactionSummaryResponse CreateDailySummaryResponse(DateOnly date)
        {
            return new DailyTransactionSummaryResponse
            {
                Date = date,
                TotalTransactions = 2,
                TotalAmount = 1000.00m,
                AverageAmount = 500.00m,
                LargestTransaction = 500.00m,
                SmallestTransaction = 500.00m,
                Statuses = new List<TransactionStatusSummary>
                {
                    new TransactionStatusSummary
                    {
                        Status = TransactionStatusEnum.Pending,
                        Count = 2,
                        TotalAmount = 1000.00m
                    }
                }
            };
        }
    }
}
