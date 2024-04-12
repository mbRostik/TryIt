using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Queries;
using Chats.Infrastructure.Services.grpcServices;
using Grpc.Core;
using MediatR;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userchats;

namespace Chats.Infrastructure.grpcServices.Tests
{
    public class grpcUserChats_ServiceTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly grpcUserChats_Service _service;

        public grpcUserChats_ServiceTests()
        {
            _service = new grpcUserChats_Service(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserChats_ReturnsChats_WhenChatsExist()
        {
            // Arrange
            var request = new GetUserChatsRequest { UserId = "user1" };
            var chats = new List<GiveUserChatsDTO>
            {
                new GiveUserChatsDTO { ChatId = 1, ContactId = "user2", LastMessage = "Hello", LastMessageSenderId = "user2", LastActivity = DateTime.UtcNow }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserChatsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(chats);

            // Act
            var response = await _service.GetUserChats(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Chats);
            Assert.Equal("Hello", response.Chats[0].LastMessage);
        }

        [Fact]
        public async Task GetUserChats_ReturnsEmptyChat_WhenNoChatsExist()
        {
            // Arrange
            var request = new GetUserChatsRequest { UserId = "user3" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserChatsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((List<GiveUserChatsDTO>)null);

            // Act
            var response = await _service.GetUserChats(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Chats);
            Assert.Equal(0, response.Chats[0].ChatId);
        }

        [Fact]
        public async Task GetUserChats_CatchesExceptionsAndReturnsEmptyChat()
        {
            // Arrange
            var request = new GetUserChatsRequest { UserId = "user4" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserChatsQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var response = await _service.GetUserChats(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Chats);
            Assert.Equal(0, response.Chats[0].ChatId);
        }
    }
}
