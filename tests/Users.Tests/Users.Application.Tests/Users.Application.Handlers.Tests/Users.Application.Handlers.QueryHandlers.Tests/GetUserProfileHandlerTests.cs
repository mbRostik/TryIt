using AutoMapper;
using FluentAssertions;
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

public class GetUserProfileHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetUserProfileHandler _handler;
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly Mock<IMapperService> _mapperServiceMock = new Mock<IMapperService>();

    public GetUserProfileHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForUserProfile")
            .Options;

        _dbContext = new UserDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserProfileDTO>();
        });
        IMapper mapper = config.CreateMapper();
        _mapperServiceMock.Setup(m => m.Mapper_UserToUserProfileDTO()).Returns(mapper);

        _handler = new GetUserProfileHandler(_dbContext, _mapperServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserProfile_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var expectedUser = new User
        {
            Id = userId,
            Name = "Xto ia",
            Bio = "Developer",
            Email = "nema@gmail.com",
            NickName = "WhoKnows",
            Phone = "1234567890",
            Photo = new byte[] { 1, 2, 3 },
            IsPrivate = false
        };

        _dbContext.Users.Add(expectedUser);
        await _dbContext.SaveChangesAsync();

        var query = new GetUserProfileQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(expectedUser.Email);
        result.NickName.Should().Be(expectedUser.NickName);

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetUserProfileQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
