using Azka_Transaction_Processing_System.Application.Modules.PaymentMethods.GetPaymentMethods;
using FluentAssertions;
using Moq;
using TPS.UnitTests.Common;
using Xunit;

namespace TPS.UnitTests.UseCases
{
    public class GetPaymentMethodsUseCaseTests : TestBase
    {
        private readonly GetPaymentMethodsUseCase _sut;

        public GetPaymentMethodsUseCaseTests()
        {
            _sut = new GetPaymentMethodsUseCase(MockPaymentMethodRepo.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnAllPaymentMethods_WhenPaymentMethodsExist()
        {
            // Arrange
            var list = new List<PaymentMethodResponse>
            {
                new PaymentMethodResponse { Id = 1, Name = "Visa" },
                new PaymentMethodResponse { Id = 2, Name = "MasterCard" }
            };

            MockPaymentMethodRepo.Setup(x => x.GetAllPaymentMethodsAsync())
                .ReturnsAsync(list);

            // Act
            var result = await _sut.ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Visa");
            result[1].Name.Should().Be("MasterCard");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoPaymentMethodsExist()
        {
            // Arrange
            var emptyList = new List<PaymentMethodResponse>();
            MockPaymentMethodRepo.Setup(x => x.GetAllPaymentMethodsAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _sut.ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallGetAllPaymentMethodsAsync_Once()
        {
            // Arrange
            MockPaymentMethodRepo.Setup(x => x.GetAllPaymentMethodsAsync())
                .ReturnsAsync(new List<PaymentMethodResponse>());

            // Act
            await _sut.ExecuteAsync();

            // Assert
            MockPaymentMethodRepo.Verify(x => x.GetAllPaymentMethodsAsync(), Times.Once);
        }
    }
}
