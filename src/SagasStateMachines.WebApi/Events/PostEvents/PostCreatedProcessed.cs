using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;

namespace SagasStateMachines.WebApi.Events.PostEvents
{
    public class PostCreatedProcessed : IPostCreate_Processed
    {
        public PostCreatedProcessed(Guid correlationId, PostCreationDTO post)
        {
            Data = post;
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }

        public PostCreationDTO Data { get; }
    }
}
