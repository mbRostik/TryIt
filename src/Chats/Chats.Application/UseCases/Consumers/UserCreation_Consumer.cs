using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Domain.Entities;
using Chats.Application.UseCases.Commands;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using MessageBus.Messages.Events.IdentityServerService;

namespace Chats.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Send_To_ChatWebApi>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public UserCreation_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<IUserCreate_Send_To_ChatWebApi> context)
        {
            try
            {
                logger.Information("Starting to consume IUserCreate_Send_To_ChatWebApi for UserId: {UserId}", context.Message.Data.UserId);

                User temp = new User
                {
                    Id = context.Message.Data.UserId
                };
                await mediator.Send(new CreateUserCommand(temp));

                this.UpdateOrderState(context.Message.Data);

                await context.Publish<IUserCreate_SendEvent_From_ChatWebApi>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Data = context.Message.Data
                });

                logger.Information("Successfully consumed IUserCreate_Send_To_ChatWebApi and created user with UserId: {UserId}", context.Message.Data.UserId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error consuming IUserCreate_Send_To_ChatWebApi for UserId: {UserId}", context.Message.Data.UserId);
                throw; 
            }
        }
        private void UpdateOrderState(UserCreationDTO user) =>
           user.Status = UserCreationStatuses.ChatWebApi_Created;
    }
}
