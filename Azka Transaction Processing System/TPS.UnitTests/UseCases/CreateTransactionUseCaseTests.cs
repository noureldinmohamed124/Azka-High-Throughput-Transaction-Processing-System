using Azka_Transaction_Processing_System.Application.Common.DTOs;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using FluentAssertions;
using Moq;
using TPS.UnitTests.Common;
using TPS.UnitTests.TestData;
using Xunit;

namespace TPS.UnitTests.UseCases
{
    public class CreateTransactionUseCaseTests : TestBase
    {
        private readonly CreateTransactionUseCase _sut;

        public CreateTransactionUseCaseTests()
        {
            _sut = new CreateTransactionUseCase(
                MockCustomerRepo.Object,
                MockBranchRepo.Object,
                MockPaymentMethodRepo.Object,
                MockTransactionRepo.Object,
                MockReceiptGenerator.Object,
                MockUnitOfWork.Object,
                MockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCreateTransactionSuccessfully_WhenCommandIsValid()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            var customer = CustomerTestData.CreateValidCustomer(command.CustomerId);
            var branch = BranchTestData.CreateValidBranch(command.BranchId);
            var paymentMethod = PaymentMethodTestData.CreateValidPaymentMethod(command.PaymentMethodId);
            var receiptResult = new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };

            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(customer);
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(branch);
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(paymentMethod);
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(receiptResult);

            // Act
            var result = await _sut.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.ReceiptNumber.Should().Be(receiptResult.ReceiptNumber);
            result.Amount.Should().Be(command.Amount);
            result.Status.Should().Be(command.TransactionStatus);

            MockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            MockTransactionRepo.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockUnitOfWork.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync((Customer?)null);

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(command);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Customer was not Found");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenBranchDoesNotExist()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync((Branch?)null);

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(command);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Branch was not Found");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenPaymentMethodDoesNotExist()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync((PaymentMethod?)null);

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(command);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("This Payment Method was not Found");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRetryAndSucceed_WhenDuplicateReceiptSequenceExceptionIsThrown()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            var receiptResult = new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000002", Sequence = 2, Date = DateOnly.FromDateTime(DateTime.UtcNow) };

            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(receiptResult);

            // Fail on 1st attempt with DuplicateReceiptSequenceException, succeed on 2nd attempt
            MockUnitOfWork.SetupSequence(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DuplicateReceiptSequenceException("Conflict"))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            MockUnitOfWork.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            MockUnitOfWork.Verify(x => x.ClearChanges(), Times.Once);
            MockUnitOfWork.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRollbackTransactionAndRethrow_WhenGenericExceptionOccurs()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1 });

            MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database disk error"));

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(command);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database disk error");

            MockUnitOfWork.Verify(x => x.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowBusinessRuleException_WhenMaxRetriesExceeded()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1 });

            // Always throw DuplicateReceiptSequenceException
            MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DuplicateReceiptSequenceException("Conflict"));

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(command);

            // Assert
            await action.Should().ThrowAsync<BusinessRuleException>()
                .WithMessage("Unable to generate a unique receipt number. Please try again.");

            MockUnitOfWork.Verify(x => x.RollbackTransactionAsync(), Times.Exactly(3));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetSettledOnDate_WhenProvidedInCommand()
        {
            // Arrange
            var expectedSettledDate = new DateTime(2026, 8, 2, 12, 0, 0, DateTimeKind.Utc);
            var command = TransactionTestData.CreateValidCommand();
            command.SettledOn = expectedSettledDate;

            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1 });

            Transaction? addedTransaction = null;
            MockTransactionRepo.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => addedTransaction = t)
                .Returns(Task.CompletedTask);

            // Act
            await _sut.ExecuteAsync(command);

            // Assert
            addedTransaction.Should().NotBeNull();
            addedTransaction!.SettledOn.Should().Be(expectedSettledDate);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldKeepSettledOnNull_WhenNotProvidedInCommand()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            command.SettledOn = null;

            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1 });

            Transaction? addedTransaction = null;
            MockTransactionRepo.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => addedTransaction = t)
                .Returns(Task.CompletedTask);

            // Act
            await _sut.ExecuteAsync(command);

            // Assert
            addedTransaction.Should().NotBeNull();
            addedTransaction!.SettledOn.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallUnitOfWorkCommit_WhenTransactionIsSuccessful()
        {
            // Arrange
            var command = TransactionTestData.CreateValidCommand();
            MockCurrentUserService.Setup(x => x.UserId).Returns(command.CustomerId);
            MockCustomerRepo.Setup(x => x.GetByIdAsync(command.CustomerId)).ReturnsAsync(CustomerTestData.CreateValidCustomer());
            MockBranchRepo.Setup(x => x.GetByIdAsync(command.BranchId)).ReturnsAsync(BranchTestData.CreateValidBranch());
            MockPaymentMethodRepo.Setup(x => x.GetByIdAsync(command.PaymentMethodId)).ReturnsAsync(PaymentMethodTestData.CreateValidPaymentMethod());
            MockReceiptGenerator.Setup(x => x.GenerateAsync(command.TransactionType, command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiptNumberResult { ReceiptNumber = "PAY-20260802-1-000001", Sequence = 1 });

            // Act
            await _sut.ExecuteAsync(command);

            // Assert
            MockUnitOfWork.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }
    }
}
