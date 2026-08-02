using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Abstractions.Services;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction
{
    public class CreateTransactionUseCase
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IBranchRepo _branchRepo;
        private readonly IPaymentMethodRepo _paymentMethodRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IReceiptGenerator _receiptGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTransactionUseCase(ICustomerRepo customerRepository, IBranchRepo branchRepository, IPaymentMethodRepo paymentMethodRepository, ITransactionRepo transactionRepository, IReceiptGenerator receiptGenerator, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _customerRepo = customerRepository;
            _branchRepo = branchRepository;
            _paymentMethodRepo = paymentMethodRepository;
            _transactionRepo = transactionRepository;
            _receiptGenerator = receiptGenerator;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<CreateTransactionResponse> ExecuteAsync(CreateTransactionCommand command)
        {

            var userId = _currentUserService.UserId;
            command.CustomerId = userId;

            // validate customer, branch, payment method
            await ValidateTransactionReferencesAsync(command);

            const int MaxRetries = 3;

            for (int attempt = 1;attempt <= MaxRetries; attempt++)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    
                    var receipt = await _receiptGenerator.GenerateAsync(command.TransactionType, command.CustomerId);

                    var transaction = new Transaction
                    {
                        ReceiptNumber = receipt.ReceiptNumber,
                        CustomerId = command.CustomerId,
                        BranchId = command.BranchId,
                        PaymentMethodId = command.PaymentMethodId,
                        Amount = command.Amount,
                        Status = command.TransactionStatus,
                        CreatedOn = DateTime.UtcNow,
                        SettledOn = null
                    };

                    if (command.SettledOn != null)
                        transaction.SettledOn = command.SettledOn;


                    await _transactionRepo.AddAsync(transaction);

                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();

                    return new CreateTransactionResponse
                    {
                        TransactionId = transaction.Id,
                        CreatedOn = transaction.CreatedOn,
                        ReceiptNumber = receipt.ReceiptNumber,
                        Amount = transaction.Amount,
                        Status = transaction.Status,
                    };
                }
                catch (DuplicateReceiptSequenceException)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _unitOfWork.ClearChanges();

                    continue;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }

            throw new BusinessRuleException("Unable to generate a unique receipt number. Please try again.");
        }

        private async Task ValidateTransactionReferencesAsync(CreateTransactionCommand command)
        {

            var customer = await _customerRepo.GetByIdAsync(command.CustomerId);

            if (customer == null) throw new NotFoundException("Customer was not Found");


            var branch = await _branchRepo.GetByIdAsync(command.BranchId);

            if (branch == null) throw new NotFoundException("Branch was not Found");


            var paymentMethod = await _paymentMethodRepo.GetByIdAsync(command.PaymentMethodId);

            if (paymentMethod == null) throw new NotFoundException("This Payment Method was not Found");

        }




    }
}
