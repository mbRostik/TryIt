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
    public class GetChatParticipantsHandler : IRequestHandler<GetChatParticipantsQuery, User>
    {

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public GetChatParticipantsHandler(ChatDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<User> Handle(GetChatParticipantsQuery request, CancellationToken cancellationToken)
        {
            logger.Information("Starting to handle GetChatParticipantsQuery for ChatId {ChatId} excluding SenderId {SenderId}", request.id, request.SenderId);

            var chatParticipants = dbContext.ChatParticipants.Where(a => a.ChatId == request.id && a.UserId != request.SenderId).ToArray();
            if (chatParticipants.Length == 0)
            {
                logger.Warning("No participants found for ChatId {ChatId} excluding SenderId {SenderId}", request.id, request.SenderId);
            }
            else
            {
                logger.Information("{Count} participants found for ChatId {ChatId} excluding SenderId {SenderId}", chatParticipants.Length, request.id, request.SenderId);
            }

            var result = dbContext.Users
                .Join(dbContext.ChatParticipants,
                      user => user.Id,
                      participant => participant.UserId,
                      (user, participant) => new { User = user, Participant = participant })
                .Where(up => up.Participant.ChatId == request.id && up.Participant.UserId != request.SenderId)
                .Select(up => up.User)
                .FirstOrDefault();

            if (result == null)
            {
                logger.Warning("Unable to find the other participant in chat {ChatId} excluding sender {SenderId}", request.id, request.SenderId);
                return null;
            }

            logger.Information("Successfully retrieved participant for ChatId {ChatId} excluding SenderId {SenderId}. ParticipantId: {ParticipantId}", request.id, request.SenderId, result.Id);

            return result;
        }

    }
}
