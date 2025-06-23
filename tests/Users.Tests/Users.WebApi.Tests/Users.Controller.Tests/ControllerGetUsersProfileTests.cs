using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Users.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;
using FluentAssertions;

namespace Users.Controller.Tests
{
    public class ControllerGetUsersProfileTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserController _controller;
        private readonly ClaimsPrincipal _user;

        public ControllerGetUsersProfileTests()
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
        public async Task GetUser_ShouldReturnNotFound_WhenUserIdIsNotProvided()
        {
            // Arrange
            var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext.HttpContext.User = emptyUser;

            // Act
            var result = await _controller.GetUser();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("User ID not found.");

            _loggerMock.Verify(log => log.Warning("GetUser called but userId is null or empty."), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), default))
                         .ReturnsAsync((UserProfileDTO)null);

            // Act
            var result = await _controller.GetUser();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("There is no information for the given user ID.");

            _loggerMock.Verify(log => log.Warning("User with ID {UserId} not found.", "12345"), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenUserFound()
        {
            // Arrange
            var expectedUserProfile = new UserProfileDTO { Name = "John Doe"};
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), default))
                         .ReturnsAsync(expectedUserProfile);

            // Act
            var result = await _controller.GetUser();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<UserProfileDTO>().Should().BeEquivalentTo(expectedUserProfile);

            _loggerMock.Verify(log => log.Information("User with ID {UserId} retrieved successfully.", "12345"), Times.Once);
        }
    }
}
