using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.PostService;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Moq;
using Serilog;
using System;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Consumers;
using Xunit;

public class PostCreationConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly PostCreation_Consumer _consumer;

    public PostCreationConsumerTests()
    {
        _consumer = new PostCreation_Consumer(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldHandlePostCreation()
    {
        // Arrange
        int postId = 1;
        var correlationId = Guid.NewGuid(); // Использование Guid
        var postCreationDTO = new PostCreationDTO { PostId = postId, Status = PostCreationStatuses.PostWebApi_Created };

        var mockMessage = new PostCreateSendToUserWebApiMock(correlationId, postCreationDTO);

        var contextMock = new Mock<ConsumeContext<IPostCreate_Send_To_UserWebApi>>();

        contextMock.SetupGet(x => x.Message).Returns(mockMessage);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        contextMock.As<IPublishEndpoint>().Setup(x => x.Publish(
            It.IsAny<IPostCreate_SendEvent_From_UserWebApi>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        Assert.Equal(PostCreationStatuses.UserWebApi_Created, postCreationDTO.Status);

        _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

}

public class PostCreateSendToUserWebApiMock : IPostCreate_Send_To_UserWebApi
{
    public Guid CorrelationId { get; }
    public PostCreationDTO Data { get; }

    public PostCreateSendToUserWebApiMock(Guid correlationId, PostCreationDTO data)
    {
        CorrelationId = correlationId;
        Data = data;
    }
}

