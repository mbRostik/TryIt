using MessageBus.Messages;
using MessageBus.Messages.Events.UserService;
using MessageBus.Models.DTOs;

namespace SagasStateMachines.WebApi.Events.UserEvents
{
    public class UserChangedProcessed : IUserChange_Processed
    {
        public UserChangedProcessed(Guid correlationId, UserChangeDTO user)
        {
            Data = user;
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }

        public UserChangeDTO Data { get; }
    }
}
