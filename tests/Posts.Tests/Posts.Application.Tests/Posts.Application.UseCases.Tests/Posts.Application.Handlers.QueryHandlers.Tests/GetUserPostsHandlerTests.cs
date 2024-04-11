using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Handlers.QueryHandlers;
using Posts.Application.UseCases.Queries;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Tests.Posts.Application.Handlers.QueryHandlers.Tests
{
    public class GetUserPostsHandlerTests
    {
        private readonly PostDbContext _dbContext;
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetUserPostsHandler _handler;

        public GetUserPostsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserPosts")
                .Options;

            _dbContext = new PostDbContext(options);

            _handler = new GetUserPostsHandler(_dbContext, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserPosts_WhenPostsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new GetUserPostsQuery(userId);

            var posts = new List<Post>
            {
                new Post { Id = 1, UserId = userId, Title = "Post 1", Content = "Content 1", Date = DateTime.Now },
                new Post { Id = 2, UserId = userId, Title = "Post 2", Content = "Content 2", Date = DateTime.Now }
            };

            _dbContext.Posts.AddRange(posts);
            await _dbContext.SaveChangesAsync();

            var expectedPosts = posts.Select(p => new GiveProfilePostsDTO
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Date = p.Date,
                Files = new List<GiveFileDTO>()
            }).ToList();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPosts);

            _loggerMock.Verify(log => log.Information(It.IsAny<string>(), userId, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoPostsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new GetUserPostsQuery(userId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _loggerMock.Verify(log => log.Information(It.IsAny<string>(), userId, It.IsAny<int>()), Times.Once);
        }
    }
}
