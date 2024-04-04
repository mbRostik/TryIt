using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.PostService;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Reports.Application.UseCases.Commands;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Consumers
{
    public class PostCreation_Consumer : IConsumer<IPostCreate_Send_To_ReportWebApi>
    {
        private readonly IMediator mediator;
        public PostCreation_Consumer(IMediator _mediator)
        {
            mediator = _mediator;

        }
        public async Task Consume(ConsumeContext<IPostCreate_Send_To_ReportWebApi> context)
        {
            Post temp = new Post { Id=context.Message.Data.PostId};

            await mediator.Send(new CreatePostCommand(temp));

            this.UpdatePostState(context.Message.Data);

            await context.Publish<IPostCreate_SendEvent_From_ReportWebApi>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Data = context.Message.Data
            });

            await Task.CompletedTask;
        }

        private void UpdatePostState(PostCreationDTO post) =>
          post.Status = PostCreationStatuses.ReportWebApi_Created;
    }
}