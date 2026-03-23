using FluentAssertions;
using Moq;
using UserAssessment_Application.DTOs.Request;
using UserAssessment_Application.Entities;
using UserAssessment_Application.Repositories.IRepo;
using UserAssessment_Application.Services.CoreServices;
using UserAssessment_Application.Services.ICoreServices;
namespace UserAssessment.Tests
{
    public class AuthControllerTests
    {
        // Mock dependencies injected into AuthenticationService

        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IAuthenticationService _service;

        // Constructor to set up the test environment
        public AuthControllerTests()
        {
            // Initialize the mock dependencies and the service with the mocked dependencies
            _mockUserRepo = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();

            _service = new AuthenticationService(
                _mockUserRepo.Object,
                _mockTokenService.Object
            );
        }

        // Test cases for LoginAsync and RegisterAsync methods of AuthenticationService
        [Fact]
        public async Task LoginAsync_ReturnsSuccess_WhenValidUser()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "252525"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(request.Email))
                         .ReturnsAsync(user);

            _mockTokenService.Setup(x => x.GenerateToken(user))
                             .Returns("fake-token");

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Token.Should().Be("fake-token");
        }

        // Test case for when the user is not found during login
        [Fact]
        public async Task LoginAsync_ReturnsFail_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "notfound@test.com",
                Password = "202020"
            };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(request.Email))
                         .ReturnsAsync((User)null);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        // Test case for when the password is incorrect during login
        [Fact]
        public async Task RegisterAsync_ReturnsFail_WhenEmailExists()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Password = "abcdefg"
            };

            // Set up the mock to return an existing user when checking for email existence
            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(request.Email))
                         .ReturnsAsync(new User()); 

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email is already registered");
        }
    }
}
