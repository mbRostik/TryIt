using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.PostService;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Moq;
using Reports.Application.UseCases.Commands;
using Reports.Application.UseCases.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Tests.Reports.Applications.Consumer.Tests
{
    public class PostCreationConsumerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ConsumeContext<IPostCreate_Send_To_ReportWebApi>> _contextMock = new Mock<ConsumeContext<IPostCreate_Send_To_ReportWebApi>>();
        private readonly PostCreation_Consumer _consumer;

        public PostCreationConsumerTests()
        {
            _consumer = new PostCreation_Consumer(_mediatorMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldCreatePostAndPublishEvent()
        {
            // Arrange
            var postId = 123;
            var postCreationDTO = new PostCreationDTO
            {
                PostId = postId,
                Status = PostCreationStatuses.PostWebApi_Created
            };
            var messageMock = new Mock<IPostCreate_Send_To_ReportWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(postCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _mediatorMock.Verify(x => x.Send(It.Is<CreatePostCommand>(cmd => cmd.model.Id == postId), It.IsAny<System.Threading.CancellationToken>()));
        }

        [Fact]
        public async Task Consume_ShouldHandleExceptionAndNotPublishEvent()
        {
            // Arrange
            var postId = 456;
            var postCreationDTO = new PostCreationDTO
            {
                PostId = postId,
                Status = PostCreationStatuses.PostWebApi_Created
            };
            var messageMock = new Mock<IPostCreate_Send_To_ReportWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(postCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreatePostCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            Func<Task> act = async () => await _consumer.Consume(_contextMock.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
            _contextMock.Verify(x => x.Publish(It.IsAny<IPostCreate_SendEvent_From_ReportWebApi>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        }
    }
}
