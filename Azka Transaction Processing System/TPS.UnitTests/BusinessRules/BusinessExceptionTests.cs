using Azka_Transaction_Processing_System.Application.Exceptions;
using FluentAssertions;
using System.Net;
using Xunit;

namespace TPS.UnitTests.BusinessRules
{
    public class BusinessExceptionTests
    {
        [Fact]
        public void NotFoundException_ShouldSetStatusCodeToNotFound()
        {
            // Arrange & Act
            var ex = new NotFoundException("Resource not found");

            // Assert
            ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
            ex.Message.Should().Be("Resource not found");
        }

        [Fact]
        public void BusinessRuleException_ShouldSetStatusCodeToConflict()
        {
            // Arrange & Act
            var ex = new BusinessRuleException("Sequence generation rule violated");

            // Assert
            ex.StatusCode.Should().Be(HttpStatusCode.Conflict);
            ex.Message.Should().Be("Sequence generation rule violated");
        }

        [Fact]
        public void DuplicateReceiptSequenceException_ShouldInheritFromAppException()
        {
            // Arrange & Act
            var ex = new DuplicateReceiptSequenceException("Sequence duplicate detected");

            // Assert
            ex.Should().BeAssignableTo<AppException>();
            ex.Message.Should().Be("Sequence duplicate detected");
        }
    }
}
