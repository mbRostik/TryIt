using MassTransit;
using MessageBus.Models.DTOs;
using MongoDB.Bson;

namespace SagasStateMachines.WebApi.States.UserStates
{
    public class ProcessingUserCreationState : SagaStateMachineInstance, ISagaVersion
    {
        public ProcessingUserCreationState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public ObjectId Id { get; set; }
        public UserCreationDTO Data { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CorrelationId { get; set; }

        public string State { get; set; }
        public int Version { get; set; }
    }
}
