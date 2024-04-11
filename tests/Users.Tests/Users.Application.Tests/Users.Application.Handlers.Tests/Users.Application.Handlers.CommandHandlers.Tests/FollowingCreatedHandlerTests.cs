using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;
using FluentAssertions;
using Users.Application.UseCases.Handlers.OperationHandlers;

public class FollowingCreatedHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly FollowingCreatedHandler _handler;

    public FollowingCreatedHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForFollowingCreation")
            .Options;

        _dbContext = new UserDbContext(options);

        _handler = new FollowingCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateFollowing_WhenItDoesNotExist()
    {
        // Arrange
        var followerId = Guid.NewGuid().ToString();
        var profileId = Guid.NewGuid().ToString();
        var command = new CreateFollowingCommand(followerId, profileId);
        var expectedUserProfile = new UserProfileDTO {Name = "John Doe" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserProfile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUserProfile);

        var followingEntity = await _dbContext.Follows.FirstOrDefaultAsync(x => x.FollowerId == followerId && x.UserId == profileId);
        followingEntity.Should().NotBeNull();

        _loggerMock.Verify(log => log.Information(It.IsAny<string>(), profileId), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldRemoveFollowing_WhenItExists()
    {
        // Arrange
        var followerId = Guid.NewGuid().ToString();
        var profileId = Guid.NewGuid().ToString();
        var existingFollow = new Follow { FollowerId = followerId, UserId = profileId };
        _dbContext.Follows.Add(existingFollow);
        await _dbContext.SaveChangesAsync();

        var command = new CreateFollowingCommand(followerId, profileId);
        var expectedUserProfile = new UserProfileDTO {Name = "Jane Doe" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserProfile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUserProfile);

        var followingEntity = await _dbContext.Follows.FirstOrDefaultAsync(x => x.FollowerId == followerId && x.UserId == profileId);
        followingEntity.Should().BeNull(); // Following should be removed

        _loggerMock.Verify(log => log.Information(It.IsAny<string>(), profileId), Times.AtLeastOnce);
    }
}
