using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.GetTransactionByReceipt;
using Azka_Transaction_Processing_System.Domain.Entities;
using FluentAssertions;
using Moq;
using TPS.UnitTests.Common;
using TPS.UnitTests.TestData;
using Xunit;

namespace TPS.UnitTests.UseCases
{
    public class GetTransactionByReceiptUseCaseTests : TestBase
    {
        private readonly GetTransactionByReceiptUseCase _sut;

        public GetTransactionByReceiptUseCaseTests()
        {
            _sut = new GetTransactionByReceiptUseCase(MockTransactionRepo.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTransactionDetails_WhenReceiptExists()
        {
            // Arrange
            const string receiptNumber = "PAY-20260802-1-000001";
            var query = new GetTransactionByReceiptQuery { RecieptNumber = receiptNumber };
            var transaction = TransactionTestData.CreateValidTransaction(1, receiptNumber);

            MockTransactionRepo.Setup(x => x.GetTransactionDetailsByReceiptAsync(receiptNumber))
                .ReturnsAsync(transaction);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.ReceiptNumber.Should().Be(receiptNumber);
            result.Amount.Should().Be(transaction.Amount);
            result.Status.Should().Be(transaction.Status);
            result.Customer.Id.Should().Be(transaction.Customer.Id);
            result.Customer.Name.Should().Be(transaction.Customer.FullName);
            result.Branch.Id.Should().Be(transaction.Branch.Id);
            result.Branch.Name.Should().Be(transaction.Branch.Name);
            result.PaymentMethod.Id.Should().Be(transaction.PaymentMethod.Id);
            result.PaymentMethod.Name.Should().Be(transaction.PaymentMethod.Name);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenReceiptDoesNotExist()
        {
            // Arrange
            const string receiptNumber = "NON-EXISTENT-RECEIPT";
            var query = new GetTransactionByReceiptQuery { RecieptNumber = receiptNumber };

            MockTransactionRepo.Setup(x => x.GetTransactionDetailsByReceiptAsync(receiptNumber))
                .ReturnsAsync((Transaction?)null);

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(query);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Transaction was not found.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenReceiptNumberIsEmptyOrWhitespace(string emptyReceipt)
        {
            // Arrange
            var query = new GetTransactionByReceiptQuery { RecieptNumber = emptyReceipt };
            MockTransactionRepo.Setup(x => x.GetTransactionDetailsByReceiptAsync(emptyReceipt))
                .ReturnsAsync((Transaction?)null);

            // Act
            Func<Task> action = async () => await _sut.ExecuteAsync(query);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCorrectlyMapCustomerBranchAndPaymentMethod_WhenReceiptExists()
        {
            // Arrange
            const string receiptNumber = "PAY-20260802-1-000001";
            var query = new GetTransactionByReceiptQuery { RecieptNumber = receiptNumber };
            var transaction = TransactionTestData.CreateValidTransaction(10, receiptNumber);

            MockTransactionRepo.Setup(x => x.GetTransactionDetailsByReceiptAsync(receiptNumber))
                .ReturnsAsync(transaction);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Customer.Should().NotBeNull();
            result.Branch.Should().NotBeNull();
            result.PaymentMethod.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallRepositoryGetDetailsOnce_WhenExecuted()
        {
            // Arrange
            const string receiptNumber = "PAY-20260802-1-000001";
            var query = new GetTransactionByReceiptQuery { RecieptNumber = receiptNumber };
            var transaction = TransactionTestData.CreateValidTransaction(1, receiptNumber);

            MockTransactionRepo.Setup(x => x.GetTransactionDetailsByReceiptAsync(receiptNumber))
                .ReturnsAsync(transaction);

            // Act
            await _sut.ExecuteAsync(query);

            // Assert
            MockTransactionRepo.Verify(x => x.GetTransactionDetailsByReceiptAsync(receiptNumber), Times.Once);
        }
    }
}
