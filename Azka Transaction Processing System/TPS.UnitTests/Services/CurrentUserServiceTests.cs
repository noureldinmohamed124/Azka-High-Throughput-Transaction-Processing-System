using Azka_Transaction_Processing_System.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace TPS.UnitTests.Services
{
    public class CurrentUserServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public CurrentUserServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void UserId_ShouldReturnParsedUserId_WhenValidSubClaimExists()
        {
            // Arrange
            var claims = new[] { new Claim("sub", "42") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var sut = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            var result = sut.UserId;

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void UserId_ShouldReturnParsedUserId_WhenValidNameIdentifierClaimExists()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "100") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var sut = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            var result = sut.UserId;

            // Assert
            result.Should().Be(100);
        }

        [Fact]
        public void UserId_ShouldThrowUnauthorizedAccessException_WhenNoUserIdClaimsPresent()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, "Manager") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var sut = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            Action action = () => { _ = sut.UserId; };

            // Assert
            action.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("User ID claim not found.");
        }

        [Fact]
        public void UserId_ShouldThrowUnauthorizedAccessException_WhenUserIsNull()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var sut = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            Action action = () => { _ = sut.UserId; };

            // Assert
            action.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("User ID claim not found.");
        }

        [Fact]
        public void UserId_ShouldThrowUnauthorizedAccessException_WhenClaimValueIsNotInteger()
        {
            // Arrange
            var claims = new[] { new Claim("sub", "invalid_non_integer_id") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var sut = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            Action action = () => { _ = sut.UserId; };

            // Assert
            action.Should().Throw<UnauthorizedAccessException>();
        }
    }
}
