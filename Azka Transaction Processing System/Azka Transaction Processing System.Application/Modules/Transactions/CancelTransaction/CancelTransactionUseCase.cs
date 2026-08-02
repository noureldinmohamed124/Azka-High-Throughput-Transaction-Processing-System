using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.CancelTransaction
{
    public class CancelTransactionUseCase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CancelTransactionUseCase(ITransactionRepo transactionRepo, IUnitOfWork unitOfWork)
        {
            _transactionRepo = transactionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<CancelTransactionResponse> ExecuteAsync(string receiptNumber)
        {
            var transaction = await _transactionRepo.GetByReceiptAsync(receiptNumber);

            if (transaction is null)
                throw new NotFoundException("Transaction was not found.");

            if (transaction.Status == TransactionStatusEnum.Cancelled)
                throw new BusinessRuleException("Transaction is already cancelled.");

            if (transaction.SettledOn is not null)
                throw new BusinessRuleException("Settled transactions cannot be cancelled.");

            transaction.Status = TransactionStatusEnum.Cancelled;
            transaction.CancelledOn = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new CancelTransactionResponse
            {
                ReceiptNumber = transaction.ReceiptNumber,
                Status = transaction.Status,
                CancelledOn = transaction.CancelledOn.Value
            };
        }
    }
}
