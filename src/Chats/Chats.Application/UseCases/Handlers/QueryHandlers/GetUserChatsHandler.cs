using Chats.Application.Contracts.DTOs;
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
    public class GetUserChatsHandler : IRequestHandler<GetUserChatsQuery, IEnumerable<GiveUserChatsDTO>>
    {

        private readonly ChatDbContext dbContext;

        public GetUserChatsHandler(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<GiveUserChatsDTO>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var members = await dbContext.ChatParticipants.Where(a => a.UserId == request.id).ToListAsync();
                if(!members.Any())
                {
                    return null;
                }

                var chatIds = members.Select(m => m.ChatId).Distinct().ToList();


                var filteredChatParticipants = await dbContext.ChatParticipants
                .Where(cp => chatIds.Contains(cp.ChatId) && cp.UserId != request.id)
                .ToListAsync();

                var chats = await dbContext.Chats
                .Where(chat => chatIds.Contains(chat.Id))
                .ToListAsync();

                foreach (var chat in chats)
                {
                    chat.LastMessage = await dbContext.Messages
                     .Where(m => m.ChatId == chat.Id) 
                     .OrderByDescending(m => m.Date) 
                     .FirstOrDefaultAsync();
                }

                List<GiveUserChatsDTO> result = new List<GiveUserChatsDTO>();

                for (int i=0;i!=chats.Count;i++)
                {

                    GiveUserChatsDTO temp = new GiveUserChatsDTO();

                    temp.ChatId = chats[i].Id;
                    temp.ContactId = filteredChatParticipants[i].UserId;
                    temp.LastActivity = null;
                    temp.LastMessage = "";
                    if (chats[i].LastMessage != null)
                    {
                        temp.LastActivity = chats[i].LastMessage.Date;
                        temp.LastMessage = chats[i].LastMessage.Content;
                        temp.LastMessageSenderId = chats[i].LastMessage.SenderId;
                    }

                    result.Add(temp);
                }
                return result;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


    }
}
