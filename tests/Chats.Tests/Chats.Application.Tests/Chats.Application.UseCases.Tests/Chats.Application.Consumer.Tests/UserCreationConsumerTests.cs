using MediatR;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Application.UseCases.Consumers;
using MassTransit;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Chats.Application.UseCases.Commands;
using MessageBus.Messages.Events.IdentityServerService;
namespace Chats.Application.UseCases.Tests.Chats.Application.Consumer.Tests
{
    public class UserCreationConsumerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly UserCreation_Consumer _consumer;
        private readonly Mock<ConsumeContext<IUserCreate_Send_To_ChatWebApi>> _contextMock = new Mock<ConsumeContext<IUserCreate_Send_To_ChatWebApi>>();

        public UserCreationConsumerTests()
        {
            _consumer = new UserCreation_Consumer(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldProcessUserCreation()
        {
            // Arrange
            var userId = "user123";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.UserWebApi_Created
            };
            var userCreationMessageMock = new Mock<IUserCreate_Send_To_ChatWebApi>();
            userCreationMessageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            userCreationMessageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(userCreationMessageMock.Object);

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _mediatorMock.Verify(x => x.Send(It.Is<CreateUserCommand>(cmd => cmd.model.Id == userId), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _loggerMock.Verify(log => log.Information("Successfully consumed IUserCreate_Send_To_ChatWebApi and created user with UserId: {UserId}", userId), Times.Once);
        }

        [Fact]
        public async Task Consume_ShouldLogErrorWhenExceptionOccurs()
        {
            // Arrange
            var userId = "user456";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.ChatWebApi_Created
            };
            var userCreationMessageMock = new Mock<IUserCreate_Send_To_ChatWebApi>();
            userCreationMessageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            userCreationMessageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(userCreationMessageMock.Object);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Test Exception"));

            // Act
            Func<Task> act = async () => await _consumer.Consume(_contextMock.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
            _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), "Error consuming IUserCreate_Send_To_ChatWebApi for UserId: {UserId}", userId), Times.Once);
        }
    }
}
