using Azka_Transaction_Processing_System.Application.Modules.Transactions.DailySummary;
using FluentAssertions;
using Moq;
using TPS.UnitTests.Common;
using TPS.UnitTests.TestData;
using Xunit;

namespace TPS.UnitTests.UseCases
{
    public class DailyTransactionSummaryUseCaseTests : TestBase
    {
        private readonly DailyTransactionSummaryUseCase _sut;

        public DailyTransactionSummaryUseCaseTests()
        {
            _sut = new DailyTransactionSummaryUseCase(MockTransactionRepo.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnDailySummary_WhenTransactionsExistForDate()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var query = new DailyTransactionSummaryQuery { Date = today };
            var expectedResponse = TransactionTestData.CreateDailySummaryResponse(today);

            MockTransactionRepo.Setup(x => x.GetDailySummaryAsync(today))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Date.Should().Be(today);
            result.TotalTransactions.Should().Be(2);
            result.TotalAmount.Should().Be(1000.00m);
            result.AverageAmount.Should().Be(500.00m);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptySummaryWithDate_WhenNoTransactionsExistForDate()
        {
            // Arrange
            var futureDate = new DateOnly(2099, 12, 31);
            var query = new DailyTransactionSummaryQuery { Date = futureDate };
            var emptySummary = new DailyTransactionSummaryResponse { Date = futureDate, TotalTransactions = 0, TotalAmount = 0 };

            MockTransactionRepo.Setup(x => x.GetDailySummaryAsync(futureDate))
                .ReturnsAsync(emptySummary);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Date.Should().Be(futureDate);
            result.TotalTransactions.Should().Be(0);
            result.TotalAmount.Should().Be(0);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCorrectlyCalculateTotalAmountAndCounts_WhenExecuted()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var query = new DailyTransactionSummaryQuery { Date = today };
            var summary = TransactionTestData.CreateDailySummaryResponse(today);

            MockTransactionRepo.Setup(x => x.GetDailySummaryAsync(today))
                .ReturnsAsync(summary);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Statuses.Should().HaveCount(1);
            result.Statuses[0].Count.Should().Be(2);
            result.Statuses[0].TotalAmount.Should().Be(1000.00m);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPassDateQueryToRepository_Correctly()
        {
            // Arrange
            var targetDate = new DateOnly(2026, 8, 2);
            var query = new DailyTransactionSummaryQuery { Date = targetDate };
            MockTransactionRepo.Setup(x => x.GetDailySummaryAsync(targetDate))
                .ReturnsAsync(new DailyTransactionSummaryResponse { Date = targetDate });

            // Act
            await _sut.ExecuteAsync(query);

            // Assert
            MockTransactionRepo.Verify(x => x.GetDailySummaryAsync(targetDate), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleDateOnlyQueries_ForOldAndFutureDates()
        {
            // Arrange
            var oldDate = new DateOnly(2000, 1, 1);
            var query = new DailyTransactionSummaryQuery { Date = oldDate };
            MockTransactionRepo.Setup(x => x.GetDailySummaryAsync(oldDate))
                .ReturnsAsync(new DailyTransactionSummaryResponse { Date = oldDate });

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Date.Should().Be(oldDate);
        }
    }
}
