using AutoMapper;
using FluentAssertions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Handlers.Creation;
using Posts.Application.UseCases.Notifications;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Posts.Application.UseCases.Tests.Posts.Application.Handlers.Creation.Tests
{
    public class PostCreatedHandlerTests
    {
        private readonly PostDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly PostCreatedHandler _handler;

        public PostCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForPostCreation")
                .Options;

            _dbContext = new PostDbContext(options);

            _handler = new PostCreatedHandler(_dbContext, _mediatorMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreatePost_WhenCalledWithValidRequest()
        {
            // Arrange
            var UserId = Guid.NewGuid().ToString();
            var command = new CreatePostCommand(new CreatePostDTO { UserId = UserId, Title = "Title", Content = "Content" });

            _mapperMock.Setup(m => m.InitializeAutomapper_CreatePostDTO_To_Post()).Returns(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatePostDTO, Post>();
            }).CreateMapper());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var createdPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.UserId == UserId);
            createdPost.Should().NotBeNull();
            createdPost.Title.Should().Be(command.model.Title);
            createdPost.Content.Should().Be(command.model.Content);

            _loggerMock.Verify(log => log.Information("Post with ID {PostId} created successfully", createdPost.Id), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PostCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenExceptionThrown()
        {
            // Arrange
            var postId = Guid.NewGuid().ToString();
            var command = new CreatePostCommand(new CreatePostDTO {Content = "Content" });


            _mapperMock.Setup(m => m.InitializeAutomapper_CreatePostDTO_To_Post()).Returns(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatePostDTO, Post>();
            }).CreateMapper());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            var createdPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.UserId == postId);
            createdPost.Should().BeNull();

            _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), "Error creating post. {ErrorMessage}", It.IsAny<string>()), Times.Once);
        }
    }
}
