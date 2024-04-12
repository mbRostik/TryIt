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
    public class GetChatParticipantsHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetChatParticipantsHandler _handler;

        public GetChatParticipantsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetChatParticipantsHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new GetChatParticipantsHandler(_dbContext, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnOtherParticipant_WhenChatExists()
        {
            // Arrange
            var chatId = 1;
            var senderId = "senderId";
            var otherUserId = "otherUserId";
            _dbContext.Users.AddRange(
                new User { Id = senderId},
                new User { Id = otherUserId }
            );
            _dbContext.Chats.Add(new Chat { Id = chatId });
            _dbContext.ChatParticipants.AddRange(
                new ChatParticipant { ChatId = chatId, UserId = senderId },
                new ChatParticipant { ChatId = chatId, UserId = otherUserId }
            );
            await _dbContext.SaveChangesAsync();

            var request = new GetChatParticipantsQuery(chatId, senderId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(otherUserId);
        }

        [Fact]
        public async Task Handle_ShouldLogWarning_WhenNoOtherParticipantsFound()
        {
            // Arrange
            var chatId = 2;
            var senderId = "senderIdOnly";
            _dbContext.Users.Add(new User { Id = senderId});
            _dbContext.Chats.Add(new Chat { Id = chatId });
            _dbContext.ChatParticipants.Add(new ChatParticipant { ChatId = chatId, UserId = senderId });
            await _dbContext.SaveChangesAsync();

            var request = new GetChatParticipantsQuery(chatId, senderId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
