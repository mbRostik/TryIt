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
        private readonly Serilog.ILogger logger;

        public GetChatIdHandler(ChatDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<int> Handle(GetChatIdQuery request, CancellationToken cancellationToken)
        {
            logger.Information($"Starting to handle GetChatIdQuery for user {request.user} and receiver {request.Receiver}" );

            var chatId = dbContext.ChatParticipants
                .GroupBy(cp => cp.ChatId)
                .Where(g => g.Count() == 2)
                .Where(g => g.Any(cp => cp.UserId == request.user) && g.Any(cp => cp.UserId == request.Receiver))
                .Select(g => g.Key)
                .FirstOrDefault();

            if (chatId == null || chatId == 0)
            {
                logger.Warning($"Chat between user {request.user} and receiver {request.Receiver} does not exist. Creating a new chat.");
                chatId = await mediator.Send(new CreateChatCommand(request.user, request.Receiver));
                logger.Information($"New chat created with ID {chatId} for user {request.user} and receiver {request.Receiver}");
            }
            else
            {
                logger.Information($"Found existing chat with ID {chatId} for user {request.user} and receiver {request.Receiver}");
            }

            return chatId;
        }
    }

}