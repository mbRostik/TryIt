﻿using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.QueryHandlers
{
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesQuery, IEnumerable<Message>>
    {

        private readonly ChatDbContext dbContext;

        public GetChatMessagesHandler(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Message>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
        {
            var ChatParticipants = dbContext.ChatParticipants.Where(a=> a.ChatId == request.ChatId && a.UserId== request.UserId).ToArray();

            if (!ChatParticipants.Any()) 
            {
                return null;
            }

            var result = dbContext.Messages.Where(a => a.ChatId == request.ChatId);

            return result;
        }


    }
}
