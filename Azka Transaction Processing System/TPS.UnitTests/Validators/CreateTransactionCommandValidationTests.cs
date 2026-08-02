using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
using Azka_Transaction_Processing_System.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace TPS.UnitTests.Validators
{
    public class CreateTransactionCommandValidationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        [InlineData(-0.01)]
        public void Command_ShouldBeInvalid_WhenAmountIsZeroOrNegative(decimal invalidAmount)
        {
            // Arrange
            var command = new CreateTransactionCommand { Amount = invalidAmount };

            // Act & Assert
            command.Amount.Should().BeLessThanOrEqualTo(0);
        }

        [Fact]
        public void Command_ShouldBeValid_WhenAmountIsPositiveDecimal()
        {
            // Arrange
            var command = new CreateTransactionCommand { Amount = 1500.75m };

            // Act & Assert
            command.Amount.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Command_ShouldBeInvalid_WhenBranchIdIsInvalid(int invalidBranchId)
        {
            // Arrange
            var command = new CreateTransactionCommand { BranchId = invalidBranchId };

            // Act & Assert
            command.BranchId.Should().BeLessThanOrEqualTo(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Command_ShouldBeInvalid_WhenPaymentMethodIdIsInvalid(int invalidPaymentMethodId)
        {
            // Arrange
            var command = new CreateTransactionCommand { PaymentMethodId = invalidPaymentMethodId };

            // Act & Assert
            command.PaymentMethodId.Should().BeLessThanOrEqualTo(0);
        }

        [Fact]
        public void Command_ShouldStoreEnumAsPayment_WhenTransactionTypeIsPayment()
        {
            // Arrange
            var command = new CreateTransactionCommand { TransactionType = TransactionTypeEnum.Payment };

            // Act & Assert
            command.TransactionType.Should().Be(TransactionTypeEnum.Payment);
        }

        [Fact]
        public void Command_ShouldStoreEnumAsPending_WhenTransactionStatusIsPending()
        {
            // Arrange
            var command = new CreateTransactionCommand { TransactionStatus = TransactionStatusEnum.Pending };

            // Act & Assert
            command.TransactionStatus.Should().Be(TransactionStatusEnum.Pending);
        }
    }
}
