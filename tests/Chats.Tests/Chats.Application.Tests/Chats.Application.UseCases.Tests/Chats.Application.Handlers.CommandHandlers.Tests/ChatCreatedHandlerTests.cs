using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Handlers.OperationHandlers;
using Chats.Application.UseCases.Notifications;
using Chats.Infrastructure.Data;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Tests.Chats.Application.Handlers.CommandHandlers.Tests
{
    public class ChatCreatedHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ChatCreatedHandler _handler;

        public ChatCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForChatCreatedHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new ChatCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateChatAndReturnChatId()
        {
            // Arrange
            var firstUserId = "user1";
            var secondUserId = "user2";
            var command = new CreateChatCommand(firstUserId, secondUserId);

            // Act
            var chatId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            chatId.Should().BePositive();
            _dbContext.Chats.Any(c => c.Id == chatId).Should().BeTrue();
            _mediatorMock.Verify(m => m.Publish(It.IsAny<ChatCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
         
        }
    }
}
