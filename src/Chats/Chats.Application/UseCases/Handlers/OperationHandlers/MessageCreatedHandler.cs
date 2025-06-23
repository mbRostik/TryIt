using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
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
            var user = await dbContext.ChatParticipants.AsNoTracking()
                .Where(cp => cp.ChatId == request.message.ChatId && cp.UserId != request.SenderId)
                .Select(cp => dbContext.Users.FirstOrDefault(u => u.Id == cp.UserId))
                .FirstOrDefaultAsync();

            if (user != null && user.IsCheckingMessages)
            {
                var resultCheck = await CheckMessage(request.message.MessageContent);

                if (resultCheck)
                {
                    throw new Exception("This user is not allowed to send unsafe content");
                }
            }
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

        private async Task<bool> CheckMessage(string message)
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://127.0.0.1:8004")
            };

            var requestBody = new
            {
                text = message
            };

            try
            {
                var response = await httpClient.PostAsJsonAsync("/predict", requestBody);

                if (!response.IsSuccessStatusCode)
                    return false;

                var responseData = await response.Content.ReadFromJsonAsync<PredictionResponse>();

                return responseData?.Prediction == "true";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Prediction check failed: {ex.Message}");
                return false;
            }
        }
        private class PredictionResponse
        {
            public string Prediction { get; set; }
        }
    }
}
