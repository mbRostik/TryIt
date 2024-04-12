using AutoMapper;
using FluentAssertions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Handlers.OperationHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class ProfilePhotoChangedHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly ProfilePhotoChangedHandler _handler;

    public ProfilePhotoChangedHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserCreation")
        .Options;

        _dbContext = new UserDbContext(options);

        _handler = new ProfilePhotoChangedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserProfile_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var existingUser = new User { Id = userId, 
            Name = "John Doe", 
            Photo = new byte[] { 1, 2, 3 },
            Bio = "An example bio",
            Email = "example@example.com",
            NickName = "JaneD",
            Phone = "1234567890",
        };
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var newAvatarBase64 = Convert.ToBase64String(new byte[] { 4, 5, 6 });
        var command = new ChangeUserAvatarCommand(new ProfilePhotoDTO { Id = userId, Avatar = newAvatarBase64 });
        var expectedUserProfile = new UserProfileDTO { Name = "John Doe", Photo = new byte[] { 4, 5, 6 },
            Bio = "An example bio",
            Email = "example@example.com",
            NickName = "JaneD",
            Phone = "1234567890",
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserProfile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Photo.Should().BeEquivalentTo(new byte[] { 4, 5, 6 });

        _loggerMock.Verify(log => log.Information(It.IsAny<string>(), userId), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var newAvatarBase64 = Convert.ToBase64String(new byte[] { 4, 5, 6 });
        var command = new ChangeUserAvatarCommand(new ProfilePhotoDTO { Id = userId, Avatar = newAvatarBase64 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _loggerMock.Verify(log => log.Warning(It.IsAny<string>(), userId), Times.Once);
    }
}
