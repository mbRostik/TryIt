using MassTransit;
using MediatR;
using MessageBus.Messages.IdentityServerService;
using Reports.Application.UseCases.Commands;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Consumers
{
    public class UserCreatedConsumer : IConsumer<IdentityUserCreatedEvent>
    {
        private readonly IMediator mediator;
        public UserCreatedConsumer(IMediator _mediator)
        {
            mediator = _mediator;

        }
        public async Task Consume(ConsumeContext<IdentityUserCreatedEvent> context)
        {
            User temp = new User { Id = context.Message.UserId };

            await mediator.Send(new CreateUserCommand(temp));
            await Task.CompletedTask;
        }
    }
}
