using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Handlers.OperationHandlers;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class PostCreatedHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly PostCreatedHandler _handler;

    public PostCreatedHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForPostCreation")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new PostCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewPost_WhenRequestIsValid()
    {
        // Arrange
        var postCommand = new CreatePostCommand(new Post { Id = 1 , UserId = "1"});

        // Act
        var result = await _handler.Handle(postCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be("1");

        var postInDb = await _dbContext.Posts.FindAsync(result.Id);
        postInDb.Should().NotBeNull();
        postInDb.UserId.Should().Be("1");
        postInDb.Id.Should().Be(1);

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenExceptionOccurs()
    {
        // Arrange
        var postCommand = new CreatePostCommand(null); // Simulate an invalid request that leads to an exception

        // Act
        var result = await _handler.Handle(postCommand, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
