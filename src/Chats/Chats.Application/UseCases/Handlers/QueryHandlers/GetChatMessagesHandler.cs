using AutoMapper;
using Chats.Application.Contracts.DTOs;
using Chats.Application.Contracts.Interfaces;
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
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesQuery, IEnumerable<GiveUserChatMessagesDTO>>
    {
        private readonly IMapper mapper;

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public GetChatMessagesHandler(ChatDbContext dbContext, Serilog.ILogger logger, IMapperService mapperService)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            mapperService.Mapper_Message_To_GiveUserChatMessagesDTO(ref mapper);

        }

        public async Task<IEnumerable<GiveUserChatMessagesDTO>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
        {
            logger.Information($"Handling GetChatMessagesQuery for ChatId {request.ChatId} and UserId { request.UserId}");

            var chatParticipants = dbContext.ChatParticipants.Where(a => a.ChatId == request.ChatId && a.UserId == request.UserId).ToArray();

            if (!chatParticipants.Any())
            {
                logger.Warning($"No chat participants found for ChatId { request.ChatId} and UserId {request.UserId}");
                return null;
            }

            var messages = dbContext.Messages.Where(a => a.ChatId == request.ChatId);
            List<GiveUserChatMessagesDTO> result = new List<GiveUserChatMessagesDTO>();

            foreach (var message in messages)
            {
                var temp = mapper.Map<GiveUserChatMessagesDTO>(message);
                result.Add(temp);
            }

            logger.Information($"Retrieved {result.Count} messages for ChatId {request.ChatId} and UserId { request.UserId}");

            return result;
        }



    }
}
