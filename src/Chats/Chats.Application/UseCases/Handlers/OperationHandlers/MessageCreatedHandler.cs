using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
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
    public class MessageCreatedHandler : IRequestHandler<CreateMessageCommand, int>
    {
        private readonly IMediator mediator;

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public MessageCreatedHandler(ChatDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<int> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.message.ChatId == null)
                {
                    logger.Information($"Creating new chat for sender {request.SenderId} and receiver {request.message.ReceiverId}");
                    var ChatId = await mediator.Send(new CreateChatCommand(request.SenderId, request.message.ReceiverId));

                    Message message = new Message
                    {
                        ChatId = ChatId,
                        SenderId = request.SenderId,
                        Content = request.message.MessageContent
                    };

                    var result = dbContext.Messages.AddAsync(message);
                    await dbContext.SaveChangesAsync();

                    if (!result.IsCompletedSuccessfully)
                    {
                        logger.Warning($"Failed to add message for newly created chat between sender {request.SenderId} and receiver {request.message.ReceiverId}");
                        return 0;
                    }

                    logger.Information($"Message added successfully for new chat {ChatId}", ChatId);
                    return ChatId;
                }
                else
                {
                    logger.Information($"Adding message to existing chat {request.message.ChatId} for sender {request.SenderId}");
                    Message message = new Message
                    {
                        ChatId = request.message.ChatId ?? 0,
                        SenderId = request.SenderId,
                        Content = request.message.MessageContent
                    };

                    var result = dbContext.Messages.AddAsync(message);
                    await dbContext.SaveChangesAsync();

                    return request.message.ChatId ?? 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error sending message for sender {request.SenderId}");
                return 0;
            }
        }
    }
}
