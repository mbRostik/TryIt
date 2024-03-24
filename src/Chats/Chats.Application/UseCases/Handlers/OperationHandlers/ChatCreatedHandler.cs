using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Notifications;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.OperationHandlers
{
    public class ChatCreatedHandler : IRequestHandler<CreateChatCommand, int>
    {
        private readonly IMediator mediator;

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public ChatCreatedHandler(ChatDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<int> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await dbContext.Chats.AddAsync(new Chat());
                await dbContext.SaveChangesAsync();
                await mediator.Publish(new ChatCreatedNotification(request.FirstUser, request.SecondUser, result.Entity.Id), cancellationToken);

                logger.Information($"Chat created successfully with ID {result.Entity.Id} for users {request.FirstUser} and {request.SecondUser}");

                return result.Entity.Id;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error creating chat for users {request.FirstUser} and {request.SecondUser}");
                return 0;
            }
        }
    }
}