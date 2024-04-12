using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Handlers.OperationHandlers;
using Chats.Infrastructure.Data;
using FluentAssertions;
using MassTransit;
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
    public class MessageCreatedHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly MessageCreatedHandler _handler;

        public MessageCreatedHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForMessageCreatedHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new MessageCreatedHandler(_dbContext, _mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateMessageInNewChat_WhenChatIdIsNull()
        {
            // Arrange
            var senderId = "user1";
            var receiverId = "user2";
            var messageContent = "Hello, this is a new chat!";
            var createdChatId = 1;

            var command = new CreateMessageCommand(new SendMessageDTO() { ReceiverId= receiverId , ChatId= createdChatId, MessageContent= messageContent }, senderId);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateChatCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(createdChatId);

            // Act
            var resultChatId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultChatId.Should().Be(createdChatId);
            _dbContext.Messages.Any(m => m.ChatId == createdChatId && m.Content == messageContent).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldAddMessageToExistingChat_WhenChatIdIsNotNull()
        {
            // Arrange
            var chatId = 0;
            var senderId = "user1";
            var messageContent = "Hello, adding to existing chat!";
            var receiverId = "user2";
            var command = new CreateMessageCommand(new SendMessageDTO() { ReceiverId = receiverId, MessageContent = messageContent }, senderId);

            // Act
            var resultChatId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultChatId.Should().Be(chatId);
            _dbContext.Messages.Any(m => m.ChatId == chatId && m.Content == messageContent).Should().BeTrue();
        }
    }
}
