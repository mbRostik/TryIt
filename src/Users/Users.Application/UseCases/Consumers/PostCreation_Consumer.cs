using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.PostService;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;

namespace Users.Application.UseCases.Consumers
{
    public class PostCreation_Consumer : IConsumer<IPostCreate_Send_To_UserWebApi>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger _logger;

        public PostCreation_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IPostCreate_Send_To_UserWebApi> context)
        {
            _logger.Information($"Successfully consumed IPostCreate_Send_To_UserWebApi");

            Post temp = new Post { Id = context.Message.Data.PostId };
            
            await mediator.Send(new CreatePostCommand(temp));

            this.UpdatePostState(context.Message.Data);

            await context.Publish<IPostCreate_SendEvent_From_UserWebApi>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Data = context.Message.Data
            });

            await Task.CompletedTask;
        }
        private void UpdatePostState(PostCreationDTO post) =>
           post.Status = PostCreationStatuses.UserWebApi_Created;
    }
}