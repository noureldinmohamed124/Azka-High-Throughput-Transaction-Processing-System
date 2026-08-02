using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Repositories
{
    public class TransactionRepo : GenericRepo<Transaction>, ITransactionRepo
    {
        public TransactionRepo(TPSDbContext context) : base(context)
        {
        }

        public async Task<Transaction?> GetTransactionDetailsByReceiptAsync(string receiptNumber)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Branch)
                .Include(x => x.PaymentMethod)
                .FirstOrDefaultAsync(x => x.ReceiptNumber == receiptNumber);
        }

        public async Task<IReadOnlyList<SearchTransactionSummaryResponse>> SearchAsync(int? customerId, DateOnly? date)
        {
            var query = _context.Transactions
                .AsNoTracking();

            if (customerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.CreatedOn) == date.Value);
            }

            return await query.OrderByDescending(x => x.CreatedOn)
                .Select(x => new SearchTransactionSummaryResponse
                {
                    ReceiptNumber = x.ReceiptNumber,
                    Amount = x.Amount,
                    Status = x.Status,
                    CreatedOn = x.CreatedOn,
                    CustomerName = x.Customer.FullName,
                    BranchName = x.Branch.Name,
                    PaymentMethodName = x.PaymentMethod.Name
                })
                .ToListAsync();
        }

        public async Task<DailyTransactionSummaryResponse> GetDailySummaryAsync(DateOnly date)
        {
            var query = _context.Transactions.AsNoTracking().Where(x => DateOnly.FromDateTime(x.CreatedOn) == date);

            var summary = await query.GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount),
                    AverageAmount = g.Average(x => x.Amount),
                    LargestTransaction = g.Max(x => x.Amount),
                    SmallestTransaction = g.Min(x => x.Amount)
                })
                .FirstOrDefaultAsync();


            if (summary == null)
            {
                return new DailyTransactionSummaryResponse
                {
                    Date = date
                };
            }

            var statuses = await query.GroupBy(x => x.Status)
                .Select(g => new TransactionStatusSummary
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            return new DailyTransactionSummaryResponse
            {
                Date = date,
                TotalTransactions = summary.TotalTransactions,
                TotalAmount = summary.TotalAmount,
                AverageAmount = summary.AverageAmount,
                LargestTransaction = summary.LargestTransaction,
                SmallestTransaction = summary.SmallestTransaction,
                Statuses = statuses
            };
        }

        public Task<Transaction?> GetByReceiptAsync(string receiptNumber)
        {
            throw new NotImplementedException();
        }
    }
}
