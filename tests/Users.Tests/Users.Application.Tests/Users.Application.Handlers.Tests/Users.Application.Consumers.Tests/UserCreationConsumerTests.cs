using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Moq;
using Serilog;
using System;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Consumers;
using Xunit;

public class UserCreationConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly UserCreation_Consumer _consumer;

    public UserCreationConsumerTests()
    {
        _consumer = new UserCreation_Consumer(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldHandleUserCreation()
    {
        // Arrange
        var userId = "dfqfq";
        var userEmail = "test@example.com";
        var userName = "TestUser";

        var userCreationDTO = new UserCreationDTO
        {
            UserId = userId,
            UserEmail = userEmail,
            UserName = userName,
            Status = UserCreationStatuses.IdentityServer_Created
        };

        var mockMessage = new Mock<ConsumeContext<IUserCreate_Send_To_UserWebApi>>();
        var userCreateSendToUserWebApiMock = new UserCreateSendToUserWebApiMock(Guid.NewGuid(), userCreationDTO);

        mockMessage.SetupGet(x => x.Message).Returns(userCreateSendToUserWebApiMock);

        // Act
        await _consumer.Consume(mockMessage.Object);

        // Assert
        _mediatorMock.Verify(x => x.Send(It.Is<CreateUserCommand>(cmd =>
            cmd.model.Id == userId &&
            cmd.model.Email == userEmail &&
            cmd.model.NickName == userName),
            It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }
}
public class UserCreateSendToUserWebApiMock : IUserCreate_Send_To_UserWebApi
{
    public Guid CorrelationId { get; }
    public UserCreationDTO Data { get; }

    public UserCreateSendToUserWebApiMock(Guid correlationId, UserCreationDTO data)
    {
        CorrelationId = correlationId;
        Data = data;
    }
}