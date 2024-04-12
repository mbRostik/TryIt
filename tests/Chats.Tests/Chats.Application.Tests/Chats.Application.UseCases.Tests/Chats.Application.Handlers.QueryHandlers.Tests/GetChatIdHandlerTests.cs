using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Handlers.QueryHandlers;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Chats.Application.UseCases.Tests.Chats.Application.Handlers.QueryHandlers.Tests
{
    public class GetChatIdHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetChatIdHandler _handler;

        public GetChatIdHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetChatIdHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new GetChatIdHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnExistingChatId_WhenChatExists()
        {
            // Arrange
            int expectedChatId = 1;
            var user = Guid.NewGuid().ToString();
            var receiver = Guid.NewGuid().ToString();
            var chatParticipants = new[]
            {
                new ChatParticipant { ChatId = expectedChatId, UserId = user },
                new ChatParticipant { ChatId = expectedChatId, UserId = receiver }
            };

            _dbContext.ChatParticipants.AddRange(chatParticipants);
            await _dbContext.SaveChangesAsync();

            var request = new GetChatIdQuery(user, receiver);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(expectedChatId);
        }

        [Fact]
        public async Task Handle_ShouldCreateNewChat_WhenChatDoesNotExist()
        {
            // Arrange
            int expectedNewChatId = 2;
            var user = Guid.NewGuid().ToString();
            var receiver = Guid.NewGuid().ToString();

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateChatCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedNewChatId);

            var request = new GetChatIdQuery(user, receiver);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(expectedNewChatId);
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateChatCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
