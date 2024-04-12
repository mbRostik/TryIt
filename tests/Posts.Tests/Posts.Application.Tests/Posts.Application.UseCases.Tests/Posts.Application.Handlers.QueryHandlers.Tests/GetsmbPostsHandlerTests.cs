using AutoMapper;
using FluentAssertions;
using MediatR;
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
using System.Threading;
using Xunit;

namespace Posts.Application.UseCases.Tests.Posts.Application.Handlers.QueryHandlers.Tests
{
    public class GetsmbPostsHandlerTests
    {
        private readonly PostDbContext _dbContext;
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetsmbPostsHandler _handler;

        public GetsmbPostsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetsmbPostsHandler")
                .Options;

            _dbContext = new PostDbContext(options);

            _handler = new GetsmbPostsHandler(_dbContext, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPosts_WhenPostsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new GetsmbPostsQuery(userId);

            var posts = new List<Post>
            {
                new Post { UserId = userId, Title = "Post 1", Content = "Content 1", Date = DateTime.Now },
                new Post { UserId = userId, Title = "Post 2", Content = "Content 2", Date = DateTime.Now }
            };

            _dbContext.Posts.AddRange(posts);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(posts.Count);

            for (int i = 0; i < posts.Count; i++)
            {
                var expectedPost = posts[i];
                var actualPost = result[i];

                expectedPost.Title.Should().Be(actualPost.Title);
                expectedPost.Content.Should().Be(actualPost.Content);
                expectedPost.Date.Should().Be(actualPost.Date);
            }

            _loggerMock.Verify(log => log.Information(It.IsAny<string>(), userId, It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
