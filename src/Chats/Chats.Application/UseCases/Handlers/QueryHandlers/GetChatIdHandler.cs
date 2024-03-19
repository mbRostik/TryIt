using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.QueryHandlers
{
    public class GetChatIdHandler : IRequestHandler<GetChatIdQuery, int>
    {

        private readonly ChatDbContext dbContext;
        private readonly IMediator mediator;

        public GetChatIdHandler(ChatDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<int> Handle(GetChatIdQuery request, CancellationToken cancellationToken)
        {
            var chatId = dbContext.ChatParticipants
                .GroupBy(cp => cp.ChatId)
                .Where(g => g.Count() == 2)
                .Where(g => g.Any(cp => cp.UserId == request.user) && g.Any(cp => cp.UserId == request.Receiver))
                .Select(g => g.Key)
                .FirstOrDefault(); 

            if(chatId==null || chatId == 0)
            {
                chatId = await mediator.Send(new CreateChatCommand(request.user, request.Receiver));

            }
            return chatId;
        }
    }

}