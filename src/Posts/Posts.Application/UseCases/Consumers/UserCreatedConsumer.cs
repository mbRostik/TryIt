using MassTransit;
using MediatR;
using MessageBus.Messages.IdentityServerService;
using Posts.Application.UseCases.Commands;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Consumers
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
            logger.Information("Consuming IdentityUserCreatedEvent for UserId {UserId}", context.Message.UserId);

            try
            {
                User temp = new User
                {
                    Id = context.Message.UserId
                };

                await mediator.Send(new CreateUserCommand(temp));
                logger.Information("Successfully created User for UserId {UserId}", context.Message.UserId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error consuming IdentityUserCreatedEvent for UserId {UserId}", context.Message.UserId);
            }
        }
    }
}
