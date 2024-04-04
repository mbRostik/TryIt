using MassTransit;
using MediatR;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;

namespace Users.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Send_To_UserWebApi>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger _logger;
        public UserCreation_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IUserCreate_Send_To_UserWebApi> context)
        {
            _logger.Information($"Successfully consumed IUserCreate_Send_To_UserWebApi");

            User temp = new User
            { 
                Id = context.Message.Data.UserId, 
                Email=context.Message.Data.UserEmail, 
                NickName = context.Message.Data.UserName,
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
            this.UpdateUserState(context.Message.Data);

            await context.Publish<IUserCreate_SendEvent_From_UserWebApi>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Data = context.Message.Data
            });
            await Task.CompletedTask;
        }
        private void UpdateUserState(UserCreationDTO user) =>
           user.Status = UserCreationStatuses.UserWebApi_Created;
    }
}
