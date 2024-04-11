using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Handlers.OperationHandlers;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Posts.Application.UseCases.Tests.Posts.Application.Handlers.OperationHandlers.Tests
{
    public class PostDeletedHandlerTests
    {
        private readonly PostDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly PostDeletedHandler _handler;

        public PostDeletedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForPostDeletion")
                .Options;

            _dbContext = new PostDbContext(options);

            _handler = new PostDeletedHandler(_dbContext, _mediatorMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeletePostAndFiles_WhenCalledWithValidRequest()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var postId = Guid.NewGuid().ToString();

            var post = new Post { UserId = userId, Content="Efe", Title="efef"};
            var addedPost = _dbContext.Posts.Add(post);

            var pFile1 = new PFile {  PostId = addedPost.Entity.Id, file=new byte[1], Name="Jyued" };
            var pFile2 = new PFile { PostId = addedPost.Entity.Id, file = new byte[1], Name = "Jyueddd" };

            _dbContext.PFiles.AddRange(new List<PFile> { pFile1, pFile2 });

            await _dbContext.SaveChangesAsync();

            var command = new DeletePostCommand(addedPost.Entity.Id, userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var deletedPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == addedPost.Entity.Id);
            deletedPost.Should().BeNull();

            var deletedFiles = await _dbContext.PFiles.AnyAsync(pf => pf.PostId == addedPost.Entity.Id);
            deletedFiles.Should().BeFalse();
        }
    }
}
