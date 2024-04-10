using AutoMapper;
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
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Handlers.QueryHandlers;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using Xunit;

public class GetListUsersHandlerTests
{
    private readonly UserDbContext _dbContext;
    private readonly GetListUsersHandler _handler;
    private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

    public GetListUsersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForListUsers")
            .Options;

        _dbContext = new UserDbContext(options);
        _handler = new GetListUsersHandler(_dbContext, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserProfiles_ForExistingUsers()
    {
        // Arrange
        var userIds = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        var users = userIds.Select(id => new User { Id = id, Name = "John Doe", Bio = "fe", Email = "nema@gmail.com", NickName = "WhoKnows", Phone = "0993539682", Photo = new byte[1] }).ToList();

        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        _mapperMock.Setup(m => m.Mapper_UserToUserChatProfileDTO()).Returns(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserChatProfileDTO>();
        }).CreateMapper());

        var query = new GetListUsersQuery(userIds);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(userIds.Count);
        result.Select(u => u.NickName).Should().BeEquivalentTo(users.Select(u => u.NickName));

        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }

    [Fact]
    public async Task Handle_ShouldWarnAndSkip_ForNonExistingUsers()
    {
        // Arrange
        var userIds = new List<string> { Guid.NewGuid().ToString(), "NonExistingId" };
        var users = new List<User> {new User { Id = userIds[0], Name = "John Doe", Bio = "fe", Email = "nema@gmail.com", NickName = "WhoKnows", Phone = "0993539682", Photo = new byte[1] } };

        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        _mapperMock.Setup(m => m.Mapper_UserToUserChatProfileDTO()).Returns(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserChatProfileDTO>();
        }).CreateMapper());

        var query = new GetListUsersQuery(userIds);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        _dbContext.Database.EnsureDeleted(); // Cleanup the in-memory database
    }
}
