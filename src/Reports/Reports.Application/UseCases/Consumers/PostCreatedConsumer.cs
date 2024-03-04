using MassTransit;
using MediatR;
using MessageBus.Messages.PostService;
using Reports.Application.UseCases.Commands;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Consumers
{
    public class PostCreatedConsumer : IConsumer<PostCreatedEvent>
    {
        private readonly IMediator mediator;
        public PostCreatedConsumer(IMediator _mediator)
        {
            mediator = _mediator;

        }
        public async Task Consume(ConsumeContext<PostCreatedEvent> context)
        {
            Post temp = new Post { Id=context.Message.PostId};

            await mediator.Send(new CreatePostCommand(temp));
            await Task.CompletedTask;
        }
    }
}