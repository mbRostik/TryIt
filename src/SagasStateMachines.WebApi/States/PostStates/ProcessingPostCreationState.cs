using MassTransit;
using MessageBus.Models.DTOs;
using MongoDB.Bson;

namespace SagasStateMachines.WebApi.States.PostStates
{
    public class ProcessingPostCreationState : SagaStateMachineInstance, ISagaVersion
    {
        public ProcessingPostCreationState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public ObjectId Id { get; set; }
        public PostCreationDTO Data { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CorrelationId { get; set; }

        public string State { get; set; }
        public int Version { get; set; }
    }
}