using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Posts.Application.UseCases.Commands;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Send_To_PostWebApi>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public UserCreation_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<IUserCreate_Send_To_PostWebApi> context)
        {
            logger.Information("Consuming IdentityUserCreatedEvent for UserId {UserId}", context.Message.Data.UserId);

            try
            {
                User temp = new User
                {
                    Id = context.Message.Data.UserId
                };

                await mediator.Send(new CreateUserCommand(temp));
                logger.Information("Successfully created User for UserId {UserId}", context.Message.Data.UserId);

                this.UpdateOrderState(context.Message.Data);

                await context.Publish<IUserCreate_SendEvent_From_PostWebApi>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Data = context.Message.Data
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error consuming IdentityUserCreatedEvent for UserId {UserId}", context.Message.Data.UserId);
            }
        }

        private void UpdateOrderState(UserCreationDTO user) =>
          user.Status = UserCreationStatuses.PostWebApi_Created;
    }
}
