using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using UserAssessment_Application.Controllers;
using UserAssessment_Application.DTOs.Request;
using UserAssessment_Application.DTOs.Responce;
using UserAssessment_Application.Entities;
using UserAssessment_Application.Repositories.IRepo;

namespace UserAssessment.Tests
{
    public class UserControllerTests
    {
        // Mock dependencies injected into UserController
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserController _controller;

        // Constructor to set up the test environment
        public UserControllerTests()
        {
            // Initialize the mock repository and the controller with the mocked dependency
            _mockRepo = new Mock<IUserRepository>();
            _controller = new UserController(_mockRepo.Object);
        }

        // Test method to verify that GetUserDetails returns the correct user details when the user exists
        [Fact]
        public async Task GetUserDetails_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;

            // Create a test user to be returned by the mock repository
            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com"
            };

            _mockRepo.Setup(x => x.GetUserByIdAsync(userId))
                     .ReturnsAsync(user);

            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var userPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            // Act
            var result = await _controller.GetUserDetails();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            // Extract the ApiResponse<UserDto> from the OkObjectResult and verify its contents
            var response = okResult.Value as ApiResponse<UserDto>;

            response.Should().NotBeNull();
            response.Data.Should().NotBeNull();
            response.Data.Email.Should().Be("test@test.com");
        }

    }
}
