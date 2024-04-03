using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Notifications.Application.UseCases.Commands;
using Notifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Send_To_NotificationWebApi>
    {
        private readonly IMediator mediator;
        public UserCreation_Consumer(IMediator _mediator)
        {
            mediator = _mediator;

        }
        public async Task Consume(ConsumeContext<IUserCreate_Send_To_NotificationWebApi> context)
        {
            User temp = new User
            {
                Id = context.Message.Data.UserId
            };
            await mediator.Send(new CreateUserCommand(temp));
            await Task.CompletedTask;

            this.UpdateOrderState(context.Message.Data);

            await context.Publish<IUserCreate_SendEvent_From_NotificationWebApi>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Data = context.Message.Data
            });

        }

        private void UpdateOrderState(UserCreationDTO user) =>
          user.Status = UserCreationStatuses.NotificationWebApi_Created;
    }
}