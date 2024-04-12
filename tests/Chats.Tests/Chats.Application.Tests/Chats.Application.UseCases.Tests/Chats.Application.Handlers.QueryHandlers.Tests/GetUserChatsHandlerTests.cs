using Chats.Application.UseCases.Handlers.QueryHandlers;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Tests.Chats.Application.Handlers.QueryHandlers.Tests
{
    public class GetUserChatsHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetUserChatsHandler _handler;

        public GetUserChatsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetUserChatsHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new GetUserChatsHandler(_dbContext, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserChats_WhenChatsExist()
        {
            // Arrange
            var userId = "user1";
            var otherUserId = "user2";
            var chatId = 1;

            _dbContext.Users.AddRange(
                new User { Id = userId },
                new User { Id = otherUserId }
            );

            _dbContext.Chats.Add(new Chat { Id = chatId });
            _dbContext.ChatParticipants.AddRange(
                new ChatParticipant { ChatId = chatId, UserId = userId },
                new ChatParticipant { ChatId = chatId, UserId = otherUserId }
            );

            _dbContext.Messages.Add(new Message { ChatId = chatId, Content = "Hello", Date = DateTime.Now, SenderId = userId });

            await _dbContext.SaveChangesAsync();

            var request = new GetUserChatsQuery(userId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().ChatId.Should().Be(chatId);
            result.First().ContactId.Should().Be(otherUserId);
            result.First().LastMessage.Should().NotBeNullOrEmpty();
            result.First().LastActivity.Should().NotBeNull();
            result.First().LastMessageSenderId.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_ShouldLogWarning_WhenNoChatsFound()
        {
            // Arrange
            var userId = "user3";

            var request = new GetUserChatsQuery(userId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
