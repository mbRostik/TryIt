using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Queries;
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
    public class GetChatParticipantsHandler : IRequestHandler<GetChatParticipantsQuery, User>
    {

        private readonly ChatDbContext dbContext;

        public GetChatParticipantsHandler(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> Handle(GetChatParticipantsQuery request, CancellationToken cancellationToken)
        {
            var ChatParticipants = dbContext.ChatParticipants.Where(a => a.ChatId == request.id && a.UserId!= request.SenderId).ToArray();
            var result = dbContext.Users
            .Join(dbContext.ChatParticipants,
                  user => user.Id,
                  participant => participant.UserId,
                  (user, participant) => new { User = user, Participant = participant })
            .Where(up => up.Participant.ChatId == request.id)
            .Select(up => up.User)
            .FirstOrDefault();

            if (result==null)
            {
                return null;
            }
            return result;
        }
    }
}
