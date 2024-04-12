using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Subscriptions.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subscriptions.Application.UseCases.Handlers.OperationHandlers;
using Subscriptions.Application.UseCases.Consumers;
using Subscriptions.Application.UseCases.Commands;
using Subscriptions.Domain.Entities;
using FluentAssertions;
using MassTransit;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using MessageBus.Messages.Events.IdentityServerService;
namespace Subscriptions.Application.UseCases.Tests.Subscriptions.Applications.Consumer.Tests
{
    public class UserCreationConsumerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ConsumeContext<IUserCreate_Send_To_SubscriptionWebApi>> _contextMock = new Mock<ConsumeContext<IUserCreate_Send_To_SubscriptionWebApi>>();
        private readonly UserCreation_Consumer _consumer;

        public UserCreationConsumerTests()
        {
            _consumer = new UserCreation_Consumer(_mediatorMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldCreateUser()
        {
            // Arrange
            var userId = "user123";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.ReportWebApi_Created
            };
            var messageMock = new Mock<IUserCreate_Send_To_SubscriptionWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _mediatorMock.Verify(x => x.Send(It.Is<CreateUserCommand>(cmd => cmd.model.Id == userId), It.IsAny<System.Threading.CancellationToken>()));
        }

        [Fact]
        public async Task Consume_ShouldHandleExceptionAndNotPublishEvent()
        {
            // Arrange
            var userId = "user456";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.ReportWebApi_Created
            };
            var messageMock = new Mock<IUserCreate_Send_To_SubscriptionWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            Func<Task> act = async () => await _consumer.Consume(_contextMock.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
            _contextMock.Verify(x => x.Publish(It.IsAny<IUserCreate_SendEvent_From_SubscriptionWebApi>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        }
    }
}
