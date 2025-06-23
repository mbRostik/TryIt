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
using Users.WebApi.Controllers;

namespace Users.Controller.Tests
{
    public class ControllerChangeUserSettingsTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserController _controller;
        private readonly ClaimsPrincipal _user;

        public ControllerChangeUserSettingsTests()
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
        public async Task ChangeUserSettings_ShouldReturnOk_WhenCommandExecutionSuccessful()
        {
            // Arrange
            var model = new ChangeProfileInformationDTO
            {
                Id = "fwsgv",
                Name = "A Nema",
                NickName = "Rostik",
                Email = "fqe@gmail.com",
                Phone = "",
                Bio = "dbfg",
                Photo = [],
                DateOfBirth = DateTime.Now,
                IsPrivate = false,

                SexId = "UnIdentify"
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUserInformationCommand>(), default))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeUserSettings(model);

            // Assert
            result.Should().BeOfType<OkResult>();

            _loggerMock.Verify(log => log.Information("Starting ChangeUserSettings for user {UserId}.", "12345"), Times.Once);
            _loggerMock.Verify(log => log.Information("Successfully changed settings for user {UserId}.", "12345"), Times.Once);
        }

        [Fact]
        public async Task ChangeUserSettings_ShouldReturnInternalServerError_WhenCommandExecutionFails()
        {
            // Arrange
            var model = new ChangeProfileInformationDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUserInformationCommand>(), default))
                         .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ChangeUserSettings(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
