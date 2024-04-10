using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Handlers.QueryHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class GetSmbProfileHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetSmbProfileHandler _handler;
    private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public GetSmbProfileHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserProfile")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetSmbProfileHandler(_dbContext, _mapperMock.Object, _loggerMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserProfile_WhenProfileExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var profileId = userId;

        var user = new User
        {
            Id = userId,
            Email = "user@example.com",
            NickName = "UserNick",
            Name = "UserName",
            Phone = "123456789",
            Bio = "User Bio",
            Photo = new byte[0],
            DateOfBirth = DateTime.Now.AddYears(-20),
            IsPrivate = false
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new GetSmbProfileQuery(userId, profileId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.NickName.Should().Be(user.NickName);

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldLogAndReturnNull_OnException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var profileId = "InvalidId"; 

        var request = new GetSmbProfileQuery(profileId, userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
