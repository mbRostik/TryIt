using MediatR;
using Moq;
using Reports.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Reports.Application.UseCases.Commands;
using Reports.Application.UseCases.Handlers.Creation;
using Reports.Domain;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace Reports.Application.UseCases.Tests.Reports.Application.Handlers.CommandHandlers.Tests
{
    public class PostCreatedHandlerTests
    {
        private readonly ReportDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly PostCreatedHandler _handler;

        public PostCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ReportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForPostCreatedHandler")
                .Options;

            _dbContext = new ReportDbContext(options);
            _handler = new PostCreatedHandler(_dbContext, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreatePostAndReturnPost()
        {
            // Arrange
            var newPost = new Post { Id =1 };
            var command = new CreatePostCommand(newPost);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            _dbContext.Posts.Any(p => p.Id == result.Id).Should().BeTrue();
        }
    }
}
