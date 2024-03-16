using MassTransit;
using MediatR;
using MessageBus.Messages.IdentityServerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;

namespace Users.Application.UseCases.Consumers
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
                Id = context.Message.UserId, 
                Email=context.Message.UserEmail, 
                NickName = context.Message.UserName,
                Name=" ",
                Phone=" ",
                Bio=" ",
                Photo = [],
                DateOfBirth = DateTime.Now,
                SexId=1,
                IsBanned=false,
                IsPrivate=false
            };
            await mediator.Send(new CreateUserCommand(temp));
            await Task.CompletedTask;
        }
    }
}
