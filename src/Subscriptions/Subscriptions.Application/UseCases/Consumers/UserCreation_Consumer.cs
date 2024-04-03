using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using Subscriptions.Application.UseCases.Commands;
using Subscriptions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Send_To_SubscriptionWebApi>
    {
        private readonly IMediator mediator;
        public UserCreation_Consumer(IMediator _mediator)
        {
            mediator = _mediator;

        }
        public async Task Consume(ConsumeContext<IUserCreate_Send_To_SubscriptionWebApi> context)
        {
            User temp = new User
            {
                Id = context.Message.Data.UserId
            };
            await mediator.Send(new CreateUserCommand(temp));

            this.UpdateOrderState(context.Message.Data);

            await context.Publish<IUserCreate_SendEvent_From_SubscriptionWebApi>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Data = context.Message.Data
            });

            await Task.CompletedTask;
        }

        private void UpdateOrderState(UserCreationDTO user) =>
          user.Status = UserCreationStatuses.SubscriptionWebApi_Created;
    }
}