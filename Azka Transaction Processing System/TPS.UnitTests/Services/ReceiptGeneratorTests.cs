using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Azka_Transaction_Processing_System.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TPS.UnitTests.Services
{
    public class ReceiptGeneratorTests
    {
        private readonly Mock<IReceiptSequenceRepo> _mockReceiptSequenceRepo;
        private readonly TPSDbContext _dbContext;
        private readonly ReceiptGenerator _sut;

        public ReceiptGeneratorTests()
        {
            _mockReceiptSequenceRepo = new Mock<IReceiptSequenceRepo>();
            
            // Create in-memory db context for isolation without SQL Server
            var options = new DbContextOptionsBuilder<TPSDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new TPSDbContext(options);
            _sut = new ReceiptGenerator(_dbContext, _mockReceiptSequenceRepo.Object);
        }

        [Fact]
        public async Task GenerateAsync_ShouldCreateNewSequenceWithSequenceOne_WhenNoExistingSequenceFound()
        {
            // Arrange
            const TransactionTypeEnum prefix = TransactionTypeEnum.Payment;
            const int userId = 1;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            _mockReceiptSequenceRepo.Setup(x => x.GetForUpdateAsync(prefix, today, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReceiptSequence?)null);

            // Act
            var result = await _sut.GenerateAsync(prefix, userId);

            // Assert
            result.Should().NotBeNull();
            result.Sequence.Should().Be(1);
            result.Date.Should().Be(today);
            result.ReceiptNumber.Should().Contain("PAY-");
            result.ReceiptNumber.Should().Contain($"-{userId}-000001");

            _mockReceiptSequenceRepo.Verify(x => x.AddAsync(It.IsAny<ReceiptSequence>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateAsync_ShouldIncrementSequence_WhenExistingSequenceFound()
        {
            // Arrange
            const TransactionTypeEnum prefix = TransactionTypeEnum.Payment;
            const int userId = 1;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existingSequence = new ReceiptSequence
            {
                Id = 1,
                Prefix = prefix,
                Date = today,
                LastSequence = 15
            };

            _mockReceiptSequenceRepo.Setup(x => x.GetForUpdateAsync(prefix, today, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSequence);

            // Act
            var result = await _sut.GenerateAsync(prefix, userId);

            // Assert
            result.Should().NotBeNull();
            result.Sequence.Should().Be(16);
            result.ReceiptNumber.Should().Contain("-000016");

            _mockReceiptSequenceRepo.Verify(x => x.Update(It.Is<ReceiptSequence>(s => s.LastSequence == 16)), Times.Once);
        }

        [Fact]
        public async Task GenerateAsync_ShouldFormatReceiptNumberCorrectly_WithPrefixDateUserIdAndSequence()
        {
            // Arrange
            const TransactionTypeEnum prefix = TransactionTypeEnum.Refund;
            const int userId = 42;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existingSequence = new ReceiptSequence
            {
                Prefix = prefix,
                Date = today,
                LastSequence = 99
            };

            _mockReceiptSequenceRepo.Setup(x => x.GetForUpdateAsync(prefix, today, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSequence);

            // Act
            var result = await _sut.GenerateAsync(prefix, userId);

            // Assert
            result.ReceiptNumber.Should().EndWith("-42-000100");
        }

        [Fact]
        public async Task GenerateAsync_ShouldCallAddAsync_WhenSequenceIsNew()
        {
            // Arrange
            _mockReceiptSequenceRepo.Setup(x => x.GetForUpdateAsync(It.IsAny<TransactionTypeEnum>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReceiptSequence?)null);

            // Act
            await _sut.GenerateAsync(TransactionTypeEnum.Payment, 1);

            // Assert
            _mockReceiptSequenceRepo.Verify(x => x.AddAsync(It.IsAny<ReceiptSequence>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateAsync_ShouldCallUpdate_WhenSequenceAlreadyExists()
        {
            // Arrange
            var sequence = new ReceiptSequence { Prefix = TransactionTypeEnum.Payment, Date = DateOnly.FromDateTime(DateTime.UtcNow), LastSequence = 5 };
            _mockReceiptSequenceRepo.Setup(x => x.GetForUpdateAsync(It.IsAny<TransactionTypeEnum>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sequence);

            // Act
            await _sut.GenerateAsync(TransactionTypeEnum.Payment, 1);

            // Assert
            _mockReceiptSequenceRepo.Verify(x => x.Update(sequence), Times.Once);
        }
    }
}
