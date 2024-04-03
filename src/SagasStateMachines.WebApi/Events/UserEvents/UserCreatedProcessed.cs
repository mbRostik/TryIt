using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;

namespace SagasStateMachines.WebApi.Events.UserEvents
{
    public class UserCreatedProcessed: IUserCreate_Processed
    {
        public UserCreatedProcessed(Guid correlationId, UserCreationDTO user)
        {
            Data = user;
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }

        public UserCreationDTO Data { get; }
    }
}
