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

        public ChatCreatedHandler(ChatDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<int> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await dbContext.Chats.AddAsync(new Chat());
                await dbContext.SaveChangesAsync();
                await mediator.Publish(new ChatCreatedNotification(request.FirstUser, request.SecondUser, result.Entity.Id), cancellationToken);

                return result.Entity.Id;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
    }
}