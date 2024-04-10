using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using Users.Application.UseCases.Handlers.QueryHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Serilog.Core;
using Moq;
using Serilog;

public class GetUserByIdHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetUserByIdHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public GetUserByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetUserByIdHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var expectedUser = new User { Id = userId, Name = "John Doe", Bio="fe", Email="nema@gmail.com", NickName="WhoKnows", Phone="0993539682", Photo=new byte[1] };

        _dbContext.Users.Add(expectedUser);
        await _dbContext.SaveChangesAsync();

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().BeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
