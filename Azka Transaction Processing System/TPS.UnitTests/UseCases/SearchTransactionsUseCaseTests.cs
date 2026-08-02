using Azka_Transaction_Processing_System.Application.Modules.Transactions.SearchTransactions;
using FluentAssertions;
using Moq;
using TPS.UnitTests.Common;
using TPS.UnitTests.TestData;
using Xunit;

namespace TPS.UnitTests.UseCases
{
    public class SearchTransactionsUseCaseTests : TestBase
    {
        private readonly SearchTransactionsUseCase _sut;

        public SearchTransactionsUseCaseTests()
        {
            _sut = new SearchTransactionsUseCase(MockTransactionRepo.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTransactionsList_WhenCustomerIdFilterProvided()
        {
            // Arrange
            var query = new SearchTransactionsQuery { CustomerId = 1, Date = null };
            var list = new List<SearchTransactionSummaryResponse> { TransactionTestData.CreateSearchSummaryResponse() };

            MockTransactionRepo.Setup(x => x.SearchAsync(query.CustomerId, query.Date))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].CustomerName.Should().Be("John Doe");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTransactionsList_WhenDateFilterProvided()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var query = new SearchTransactionsQuery { CustomerId = null, Date = date };
            var list = new List<SearchTransactionSummaryResponse> { TransactionTestData.CreateSearchSummaryResponse() };

            MockTransactionRepo.Setup(x => x.SearchAsync(query.CustomerId, query.Date))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTransactionsList_WhenCustomerAndDateFiltersProvided()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var query = new SearchTransactionsQuery { CustomerId = 1, Date = date };
            var list = new List<SearchTransactionSummaryResponse> { TransactionTestData.CreateSearchSummaryResponse() };

            MockTransactionRepo.Setup(x => x.SearchAsync(query.CustomerId, query.Date))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoMatchingTransactionsFound()
        {
            // Arrange
            var query = new SearchTransactionsQuery { CustomerId = 999, Date = DateOnly.FromDateTime(DateTime.UtcNow) };
            var emptyList = new List<SearchTransactionSummaryResponse>();

            MockTransactionRepo.Setup(x => x.SearchAsync(query.CustomerId, query.Date))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _sut.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPassParametersToRepositorySearchAsync_Correctly()
        {
            // Arrange
            var query = new SearchTransactionsQuery { CustomerId = 5, Date = new DateOnly(2026, 8, 2) };
            MockTransactionRepo.Setup(x => x.SearchAsync(5, new DateOnly(2026, 8, 2)))
                .ReturnsAsync(new List<SearchTransactionSummaryResponse>());

            // Act
            await _sut.ExecuteAsync(query);

            // Assert
            MockTransactionRepo.Verify(x => x.SearchAsync(5, new DateOnly(2026, 8, 2)), Times.Once);
        }
    }
}
