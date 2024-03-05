﻿using MassTransit;
using MediatR;
using MessageBus.Messages.IdentityServerService;
using Subscriptions.Application.UseCases.Commands;
using Subscriptions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Application.UseCases.Consumers
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
            User temp = new User
            {
                Id = context.Message.UserId
            };
            await mediator.Send(new CreateUserCommand(temp));
            await Task.CompletedTask;
        }
    }
}