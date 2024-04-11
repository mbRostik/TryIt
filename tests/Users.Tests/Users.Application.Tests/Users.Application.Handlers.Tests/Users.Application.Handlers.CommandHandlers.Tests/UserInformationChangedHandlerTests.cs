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
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Handlers.OperationHandlers;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class UserInformationChangedHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly UserInformationChangedHandler _handler;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<IMapperService> _mapperServiceMock = new Mock<IMapperService>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public UserInformationChangedHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserChange")
            .Options;

        _dbContext = new UserDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChangeProfileInformationDTO, User>();
        });
        IMapper mapper = config.CreateMapper();
        _mapperServiceMock.Setup(m => m.Mapper_ChangeUserProfileToUserDTO()).Returns(mapper);

        _handler = new UserInformationChangedHandler(_dbContext, _mediatorMock.Object, _mapperServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserInformation_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var existingUser = new User {
            Id = userId,
            Name = "John Doe",
            Bio = "Developer",
            Email = "john.doe@example.com",
            NickName = "JohnD",
            Phone = "1234567890",
            Photo = new byte[] { 1, 2, 3 },
            IsPrivate = false,
            SexId=1
        };
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var updateModel = new ChangeProfileInformationDTO
        {
            Id = userId,
            Name = "Rostik Doe",
            SexId="2"
        };

        var command = new ChangeUserInformationCommand(updateModel);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _dbContext.Users.FindAsync(userId);
        updatedUser.Should().NotBeNull();
        updatedUser.Name.Should().Be(updateModel.Name);

        _loggerMock.Verify(log => log.Information("Attempting to change user information for UserId: {UserId}", userId), Times.Once);
        _loggerMock.Verify(log => log.Information("User information successfully changed for UserId: {UserId}", userId), Times.Once);
        _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
