using MassTransit;
using MediatR;
using MessageBus.Messages.IdentityServerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Domain.Entities;
using Chats.Application.UseCases.Commands;

namespace Chats.Application.UseCases.Consumers
{
    public class UserCreatedConsumer : IConsumer<IdentityUserCreatedEvent>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public UserCreatedConsumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<IdentityUserCreatedEvent> context)
        {
            try
            {
                logger.Information("Starting to consume IdentityUserCreatedEvent for UserId: {UserId}", context.Message.UserId);

                User temp = new User
                {
                    Id = context.Message.UserId
                };
                await mediator.Send(new CreateUserCommand(temp));

                logger.Information("Successfully consumed IdentityUserCreatedEvent and created user with UserId: {UserId}", context.Message.UserId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error consuming IdentityUserCreatedEvent for UserId: {UserId}", context.Message.UserId);
                throw; 
            }
        }
    }
}
