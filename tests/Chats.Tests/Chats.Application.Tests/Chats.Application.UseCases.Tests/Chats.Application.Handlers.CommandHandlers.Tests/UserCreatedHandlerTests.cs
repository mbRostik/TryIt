using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Application.UseCases.Handlers.OperationHandlers;
using Chats.Domain.Entities;
using Chats.Application.UseCases.Commands;
using FluentAssertions;
namespace Chats.Application.UseCases.Tests.Chats.Application.Handlers.CommandHandlers.Tests
{
    public class UserCreatedHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserCreatedHandler _handler;

        public UserCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreatedHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new UserCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndReturnUser()
        {
            // Arrange
            var newUser = new User {Id="1"};
            var command = new CreateUserCommand(newUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("1");
            _dbContext.Users.Any(u => u.Id == result.Id).Should().BeTrue();
            _loggerMock.Verify(log => log.Information($"User {result.Id} created successfully"), Times.Once);
        }

        
    }
}
