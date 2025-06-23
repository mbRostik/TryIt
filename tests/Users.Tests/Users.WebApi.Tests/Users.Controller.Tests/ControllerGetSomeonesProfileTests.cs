using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;
using Users.Application.Validators;
using Users.WebApi.Controllers;

namespace Users.Controller.Tests
{
    public class ControllerGetSomeonesProfileTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserController _controller;
        private readonly ClaimsPrincipal _user;

        public ControllerGetSomeonesProfileTests()
        {
            _controller = new UserController(_mediatorMock.Object, _loggerMock.Object);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, "12345")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        [Fact]
        public async Task GetSomeonesProfile_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new SomeonesProfileDTO(); // Assuming this will fail validation
            var validator = new SomeonesProfileDTOValidator();
            _controller.ModelState.AddModelError("ProfileId", "ProfileId is required.");

            // Act
            var result = await _controller.GetSomeonesProfile(request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _loggerMock.Verify(log => log.Information(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }

        [Fact]
        public async Task GetSomeonesProfile_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            // Arrange
            var request = new SomeonesProfileDTO { ProfileId = "123" };
            var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext.HttpContext.User = emptyUser;

            // Act
            var result = await _controller.GetSomeonesProfile(request);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
            _loggerMock.Verify(log => log.Information("UploadProfilePhoto called but user ID is missing."), Times.Once);
        }

        [Fact]
        public async Task GetSomeonesProfile_ShouldReturnOk_WhenNoDataFound()
        {
            // Arrange
            var request = new SomeonesProfileDTO { ProfileId = "123" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSmbProfileQuery>(), default))
                         .ReturnsAsync((GiveSmbProfileDTO)null);

            // Act
            var result = await _controller.GetSomeonesProfile(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().Be("There is no information");
            _loggerMock.Verify(log => log.Warning("No information found for ProfileId {ProfileId}.", request.ProfileId), Times.Once);
        }

        [Fact]
        public async Task GetSomeonesProfile_ShouldReturnOk_WhenDataFound()
        {
            // Arrange
            var request = new SomeonesProfileDTO { ProfileId = "123" };
            var expectedProfile = new GiveSmbProfileDTO { Id = "123", Name = "John Doe" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSmbProfileQuery>(), default))
                         .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetSomeonesProfile(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedProfile);
            _loggerMock.Verify(log => log.Information("Successfully retrieved user data for ProfileId {ProfileId}.", request.ProfileId), Times.Once);
        }

        [Fact]
        public async Task GetSomeonesProfile_ShouldReturnServerError_OnException()
        {
            // Arrange
            var request = new SomeonesProfileDTO { ProfileId = "123" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSmbProfileQuery>(), default))
                         .ThrowsAsync(new Exception("An error occurred"));

            // Act
            var result = await _controller.GetSomeonesProfile(request);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
            _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), "An error occurred while fetching user data for ProfileId {ProfileId}.", request.ProfileId), Times.Once);
        }
    }
}
