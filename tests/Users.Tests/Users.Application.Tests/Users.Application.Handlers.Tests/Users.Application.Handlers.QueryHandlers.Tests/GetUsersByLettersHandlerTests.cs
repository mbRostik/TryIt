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

public class GetUsersByLettersHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetUsersByLettersHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public GetUsersByLettersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForLetters")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetUsersByLettersHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFilteredUsers_WhenTheyExist()
    {
        // Arrange
        string searchingField = "gmail";
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid().ToString(), NickName = "gmail3", Email="Nema@gmail.com", Bio="Nema", Name="Nema", Phone="1234567890", Photo=new byte[0], IsPrivate=false },
            new User { Id = Guid.NewGuid().ToString(), NickName = "gmail", Email="Nema2@gmail.com", Bio="Nema", Name="Nema2", Phone="0987654321", Photo=new byte[0], IsPrivate=false },
            new User { Id = Guid.NewGuid().ToString(), NickName = "XtoIa", Email="Nema3@gmail.com", Bio="Nema", Name="Nema3", Phone="1231231234", Photo=new byte[0], IsPrivate=false }
        };

        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        var query = new GetUsersByLettersQuery(searchingField, "");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.All(u => u.NickName.Contains(searchingField)).Should().BeTrue();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersMatch()
    {
        // Arrange
        string searchingField = "NonExistentUser";
        var query = new GetUsersByLettersQuery(searchingField, "");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
