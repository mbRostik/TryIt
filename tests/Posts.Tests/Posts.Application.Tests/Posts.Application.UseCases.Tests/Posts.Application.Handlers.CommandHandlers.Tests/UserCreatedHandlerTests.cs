using MediatR;
using Moq;
using Posts.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Posts.Application.UseCases.Handlers.OperationHandlers;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using Posts.Application.UseCases.Commands;
using FluentAssertions;
namespace Posts.Application.UseCases.Tests.Posts.Application.Handlers.CommandHandlers.Tests
{
    public class UserCreatedHandlerTests
    {
        private readonly PostDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserCreatedHandler _handler;

        public UserCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreation")
                .Options;

            _dbContext = new PostDbContext(options);

            _handler = new UserCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenCalledWithValidRequest()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString()};
            var command = new CreateUserCommand(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Id.Should().Be(command.model.Id);

            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            userInDb.Should().NotBeNull();

            _loggerMock.Verify(log => log.Information("User with ID {UserId} created successfully", result.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenExceptionThrown()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString()};
            var command = new CreateUserCommand(user);

            _dbContext.Users = null;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();

            _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), "Error creating user. {ErrorMessage}", It.IsAny<string>()), Times.Once);
        }
    }
}
