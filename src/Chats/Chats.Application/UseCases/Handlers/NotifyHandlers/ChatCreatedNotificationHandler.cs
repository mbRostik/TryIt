using Chats.Application.UseCases.Notifications;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.NotifyHandlers
{
    public class ChatCreatedNotificationHandler : INotificationHandler<ChatCreatedNotification>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        private readonly ChatDbContext dbContext;

        public ChatCreatedNotificationHandler(ChatDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }


        public async Task Handle(ChatCreatedNotification request, CancellationToken cancellationToken)
        {
            try
            {
                await dbContext.ChatParticipants.AddRangeAsync(
                    new ChatParticipant { ChatId = request.ChatId, UserId = request.FirstUser },
                    new ChatParticipant { ChatId = request.ChatId, UserId = request.SecondUser });
                await dbContext.SaveChangesAsync();

                logger.Information($"Successfully added chat participants for ChatId: {request.ChatId} with Users: {request.FirstUser}, {request.SecondUser}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to add chat participants for ChatId: {request.ChatId} with Users: {request.FirstUser}, {request.SecondUser}");
            }

        }
    }
}