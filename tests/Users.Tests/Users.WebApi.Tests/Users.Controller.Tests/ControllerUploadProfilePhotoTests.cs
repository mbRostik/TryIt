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
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Queries;
using Users.WebApi.Controllers;

namespace Users.Controller.Tests
{
    public class ControllerUploadProfilePhotoTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserController _controller;
        private readonly ClaimsPrincipal _user;

        public ControllerUploadProfilePhotoTests()
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
        public async Task UploadProfilePhoto_ShouldReturnUnauthorized_WhenUserIdIsNotProvided()
        {
            // Arrange
            var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext.HttpContext.User = emptyUser;

            // Act
            var result = await _controller.UploadProfilePhoto(new ProfilePhotoDTO());

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>()
                .Which.Value.Should().Be("User ID is required.");

            _loggerMock.Verify(log => log.Information("UploadProfilePhoto called but user ID is missing."), Times.Once);
        }

        [Fact]
        public async Task UploadProfilePhoto_ShouldReturnOk_WhenPhotoUpdatedSuccessfully()
        {
            // Arrange
            var profilePhotoDTO = new ProfilePhotoDTO();
            var userProfileDTO = new UserProfileDTO { Name = "John Doe" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUserAvatarCommand>(), default));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), default))
                         .ReturnsAsync(userProfileDTO);

            // Act
            var result = await _controller.UploadProfilePhoto(profilePhotoDTO);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<UserProfileDTO>().Should().BeEquivalentTo(userProfileDTO);

            _loggerMock.Verify(log => log.Information("Attempting to upload profile photo for user {UserId}.", "12345"), Times.Once);
            _loggerMock.Verify(log => log.Information("Profile photo updated successfully for user {UserId}. Fetching updated user profile.", "12345"), Times.Once);
        }

        [Fact]
        public async Task UploadProfilePhoto_ShouldReturnNotFound_WhenProfileNotUpdated()
        {
            // Arrange
            var profilePhotoDTO = new ProfilePhotoDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUserAvatarCommand>(), default));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), default))
                         .ReturnsAsync((UserProfileDTO)null);

            // Act
            var result = await _controller.UploadProfilePhoto(profilePhotoDTO);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("User profile not found.");

            _loggerMock.Verify(log => log.Warning("Failed to fetch updated profile for user {UserId} after uploading photo.", "12345"), Times.Once);
        }

        [Fact]
        public async Task UploadProfilePhoto_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var profilePhotoDTO = new ProfilePhotoDTO();
            var exception = new Exception("Error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUserAvatarCommand>(), default))
                         .ThrowsAsync(exception);

            // Act
            var result = await _controller.UploadProfilePhoto(profilePhotoDTO);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Something went wrong.");

            _loggerMock.Verify(log => log.Error(exception, "An error occurred while uploading profile photo for user {UserId}.", "12345"), Times.Once);
        }
    }
}
