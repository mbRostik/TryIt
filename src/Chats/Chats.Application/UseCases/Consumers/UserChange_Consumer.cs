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
using MessageBus.Messages.Commands.UserService;
using MessageBus.Messages.Events.UserService;

namespace Chats.Application.UseCases.Consumers
{
    public class UserChange_Consumer : IConsumer<IUserChange_Send_To_ChatWebApi>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public UserChange_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IUserChange_Send_To_ChatWebApi> context)
        {
            try
            {
                logger.Information("Starting to consume IUserChange_Send_To_ChatWebApi for UserId: {UserId}", context.Message.Data.UserId);

                User temp = new User
                {
                    Id = context.Message.Data.UserId,
                    IsCheckingMessages = context.Message.Data.IsCheckingMessages,
                };
                await mediator.Send(new ChangeUserCommand(temp));

                this.UpdateOrderState(context.Message.Data);

                await context.Publish<IUserChange_SendEvent_From_ChatWebApi>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Data = context.Message.Data
                });

                logger.Information("Successfully consumed IUserChange_Send_To_ChatWebApi and changed user with UserId: {UserId}", context.Message.Data.UserId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error consuming IUserChange_Send_To_ChatWebApi for UserId: {UserId}", context.Message.Data.UserId);
                throw;
            }
        }
        private void UpdateOrderState(UserChangeDTO user) =>
           user.Status = UserChangeStatus.ChatWebApi_Changed;
    }
}