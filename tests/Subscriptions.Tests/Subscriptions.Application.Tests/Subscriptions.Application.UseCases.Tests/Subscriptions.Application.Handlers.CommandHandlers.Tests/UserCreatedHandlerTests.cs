using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Subscriptions.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subscriptions.Application.UseCases.Handlers.OperationHandlers;
using Subscriptions.Application.UseCases.Commands;
using Subscriptions.Domain.Entities;
using FluentAssertions;
namespace Subscriptions.Application.UseCases.Tests.Subscriptions.Application.Handlers.CommandHandlers.Tests
{
    public class UserCreatedHandlerTests
    {
        private readonly SubscriptionDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly UserCreatedHandler _handler;

        public UserCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SubscriptionDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreatedHandler")
                .Options;

            _dbContext = new SubscriptionDbContext(options);
            _handler = new UserCreatedHandler(_dbContext, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndReturnUser()
        {
            // Arrange
            var newUser = new User { Id = "1" };
            var command = new CreateUserCommand(newUser);

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
