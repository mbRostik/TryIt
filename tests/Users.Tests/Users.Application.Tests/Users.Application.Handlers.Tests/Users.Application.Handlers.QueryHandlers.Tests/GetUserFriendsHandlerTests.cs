using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Handlers.QueryHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class GetUserFriendsHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetUserFriendsHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public GetUserFriendsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserFriends")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetUserFriendsHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMutualFriends_WhenTheyExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var friendId1 = Guid.NewGuid().ToString();
        var friendId2 = Guid.NewGuid().ToString();

        var users = new List<User>
        {
            new User { Id = userId, NickName = "gmail3", Email="Nema@gmail.com", Bio="Nema", Name="Nema", Phone="1234567890", Photo=new byte[0], IsPrivate=false },
            new User { Id = friendId1, NickName = "gmail", Email="Nema2@gmail.com", Bio="Nema", Name="Nema2", Phone="0987654321", Photo=new byte[0], IsPrivate=false },
            new User { Id = friendId2,  NickName = "XtoIa", Email="Nema3@gmail.com", Bio="Nema", Name="Nema3", Phone="1231231234", Photo=new byte[0], IsPrivate=false }
        };

        var follows = new List<Follow>
        {
            new Follow { UserId = friendId1, FollowerId = userId },
            new Follow { UserId = userId, FollowerId = friendId1 },
            new Follow { UserId = friendId2, FollowerId = userId },
            new Follow { UserId = userId, FollowerId = friendId2 },
            new Follow { UserId = friendId1, FollowerId = friendId2 }
        };

        _dbContext.Users.AddRange(users);
        _dbContext.Follows.AddRange(follows);
        await _dbContext.SaveChangesAsync();

        var query = new GetUserFriendsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(u => u.Id).Should().Contain(new List<string> { friendId1, friendId2 });

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoMutualFriendsExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var otherUserId = Guid.NewGuid().ToString();

        var users = new List<User>
        {
            new User { Id = userId,  NickName = "XtoIa", Email="Nema3@gmail.com", Bio="Nema", Name="Nema3", Phone="1231231234", Photo=new byte[0], IsPrivate=false },
            new User { Id = otherUserId, NickName = "gmail", Email="Nema2@gmail.com", Bio="Nema", Name="Nema2", Phone="0987654321", Photo=new byte[0], IsPrivate=false  }
        };

        var follows = new List<Follow>
        {
            new Follow { UserId = otherUserId, FollowerId = userId }
        };

        _dbContext.Users.AddRange(users);
        _dbContext.Follows.AddRange(follows);
        await _dbContext.SaveChangesAsync();

        var query = new GetUserFriendsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
