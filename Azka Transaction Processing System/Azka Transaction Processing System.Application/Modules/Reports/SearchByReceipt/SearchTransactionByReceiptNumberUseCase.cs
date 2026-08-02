using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Reports.SearchByReceipt
{
    public class SearchTransactionByReceiptNumberUseCase
    {
        private readonly ITransactionRepo _transactionRepo;

        public SearchTransactionByReceiptNumberUseCase(ITransactionRepo transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }


        public async Task<TransactionDetailsResponse> ExecuteAsync(SearchTransactionByReceiptNumberQuery query)
        {
            var transaction = await _transactionRepo.GetTransactionDetailsByReceiptAsync(query.RecieptNumber);

            if (transaction is null) throw new NotFoundException("Transaction was not found.");

            return new TransactionDetailsResponse
            {
                ReceiptNumber = transaction.ReceiptNumber,
                Amount = transaction.Amount,
                Status = transaction.Status,
                CreatedOn = transaction.CreatedOn,
                SettledOn = transaction.SettledOn,
                Customer = new CustomerDto
                {
                    Id = transaction.Customer.Id,
                    Name = transaction.Customer.FullName
                },
                Branch = new BranchDto
                {
                    Id = transaction.Branch.Id,
                    Name = transaction.Branch.Name
                },
                PaymentMethod = new PaymentMethodDto
                {
                    Id = transaction.PaymentMethod.Id,
                    Name = transaction.PaymentMethod.Name
                }
            };
        }
    }
}
