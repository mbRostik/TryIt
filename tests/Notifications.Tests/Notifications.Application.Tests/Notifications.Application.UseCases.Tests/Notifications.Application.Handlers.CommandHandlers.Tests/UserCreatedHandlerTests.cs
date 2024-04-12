using MediatR;
using Moq;
using Notifications.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notifications.Application.UseCases.Handlers.OperationHandlers;
using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Entities;
using Notifications.Application.UseCases.Commands;
using FluentAssertions;
namespace Notifications.Application.UseCases.Tests.Notifications.Application.Handlers.CommandHandlers.Tests
{
    public class UserCreatedHandlerTests
    {
        private readonly NotificationDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly UserCreatedHandler _handler;

        public UserCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreatedHandler")
                .Options;

            _dbContext = new NotificationDbContext(options);
            _handler = new UserCreatedHandler(_dbContext, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndReturnUser()
        {
            // Arrange
            var newUser = new User { Id = "1" };
            var command = new CreateUserCommand (newUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("1");
            _dbContext.Users.Any(u => u.Id == result.Id).Should().BeTrue();
            _dbContext.Database.EnsureDeleted();

        }
    }
}
