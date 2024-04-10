using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.UseCases.Handlers.QueryHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class GetListOfFollowsHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetListOfFollowsHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public GetListOfFollowsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForListOfFollows")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetListOfFollowsHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfFollows_WhenFollowsExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var followId = Guid.NewGuid().ToString();

        var follows = new List<Follow>
        {
            new Follow { UserId = followId, FollowerId = userId },
            new Follow { UserId = Guid.NewGuid().ToString(), FollowerId = userId }
        };

        _dbContext.Follows.AddRange(follows);
        await _dbContext.SaveChangesAsync();

        var query = new GetListOfFollowsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(follows.Count);
        result.All(f => f.FollowerId == userId).Should().BeTrue();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFollowsExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var query = new GetListOfFollowsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
