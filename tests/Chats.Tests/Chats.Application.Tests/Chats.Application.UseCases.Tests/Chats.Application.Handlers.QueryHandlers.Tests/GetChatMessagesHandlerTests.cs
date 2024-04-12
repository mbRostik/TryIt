using AutoMapper;
using Chats.Application.Contracts.DTOs;
using Chats.Application.Contracts.Interfaces;
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
    public class GetChatMessagesHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetChatMessagesHandler _handler;

        public GetChatMessagesHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetChatMessagesHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new GetChatMessagesHandler(_dbContext, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnChatMessages_WhenChatExists()
        {
            // Arrange
            var chatId = 1;
            var userId = "user1";
            _dbContext.Chats.Add(new Chat { Id = chatId });
            _dbContext.ChatParticipants.Add(new ChatParticipant { ChatId = chatId, UserId = userId });
            _dbContext.Messages.AddRange(
                new Message { ChatId = chatId, Content = "Message 1", SenderId= userId },
                new Message { ChatId = chatId, Content = "Message 2", SenderId = userId }
            );
            await _dbContext.SaveChangesAsync();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Message, GiveUserChatMessagesDTO>()
                   .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));
            });
            var mapper = mapperConfiguration.CreateMapper();
            _mapperMock.Setup(m => m.Mapper_Message_To_GiveUserChatMessagesDTO()).Returns(mapper);

            var request = new GetChatMessagesQuery(chatId, userId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _loggerMock.Verify(log => log.Information($"Retrieved {result.Count()} messages for ChatId {chatId} and UserId {userId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogWarning_WhenChatParticipantsDoNotExist()
        {
            // Arrange
            var chatId = 2;
            var userId = "user2";

            var request = new GetChatMessagesQuery(chatId, userId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _loggerMock.Verify(log => log.Warning($"No chat participants found for ChatId {chatId} and UserId {userId}"), Times.Once);
        }
    }
}
