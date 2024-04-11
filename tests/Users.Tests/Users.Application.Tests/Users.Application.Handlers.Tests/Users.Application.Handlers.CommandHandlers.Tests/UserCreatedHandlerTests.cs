using MediatR;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Users.Application.UseCases.Handlers.OperationHandlers;

public class UserCreatedHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly UserCreatedHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public UserCreatedHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreation")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new UserCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidRequest()
    {
        // Arrange
        var newUser = new User
        {
            Id = "1",
            Name = "Jane Doe",
            Bio = "An example bio",
            Email = "example@example.com",
            NickName = "JaneD",
            Phone = "1234567890",
            Photo = new byte[0]
        };

        var command = new CreateUserCommand(newUser);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty(); // ID should be set by EF upon saving
        result.Name.Should().Be(newUser.Name);

        // Additionally, ensure the user is actually saved to the database
        var savedUser = await _dbContext.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
