using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Queries;
using Chats.Application.Validators;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace Chats.WebApi.ChatHubSpace
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public ChatHub(ChatDbContext context, IMediator mediator, Serilog.ILogger logger)
        {
            _context = context;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task JoinChat(int chatId)
        {
            try
            {
                logger.Information("Attempting to join chat {ChatId} with connection {ConnectionId}", chatId, Context.ConnectionId);
                var connectionId = Context.ConnectionId;
                await Groups.AddToGroupAsync(connectionId, chatId.ToString());
                logger.Information("Successfully joined chat {ChatId} with connection {ConnectionId}", chatId, Context.ConnectionId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error joining chat {ChatId} with connection {ConnectionId}", chatId, Context.ConnectionId);
                throw;
            }
        }


        public async Task SendMessage(SendMessageDTO message)
        {

            try
            {
                logger.Information("Sending message from user {SenderId} to chat {ChatId}", Context.UserIdentifier, message.ChatId);
                var senderId = Context.UserIdentifier;
                await mediator.Send(new CreateMessageCommand(message, senderId));
                GiveUserChatMessagesDTO mess = new GiveUserChatMessagesDTO
                {
                    Content = message.MessageContent,
                    SenderId = senderId,
                    Date = DateTime.UtcNow,
                    ChatId = message.ChatId
                };
                await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", mess);
                logger.Information("Message sent from user {SenderId} to chat {ChatId}", Context.UserIdentifier, message.ChatId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error sending message from user {SenderId} to chat {ChatId}", Context.UserIdentifier, message.ChatId);
                throw;
            }
        }
        private static readonly Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId != null)
                {
                    logger.Information("{UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
                    userConnections[userId] = Context.ConnectionId;
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during OnConnectedAsync for user {UserId}", Context.UserIdentifier);
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId != null && userConnections.ContainsKey(userId))
                {
                    userConnections.Remove(userId);
                    logger.Information("{UserId} disconnected", userId);
                }
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during OnDisconnectedAsync for user {UserId}", Context.UserIdentifier);
                throw;
            }
        }

        public async Task CreateChat(SendMessageDTO message)
        {
            try
            {
                logger.Information("Attempting to create a chat for senderId: {SenderId} with message content: '{MessageContent}'", Context.UserIdentifier, message.MessageContent);

                if (message.MessageContent == null)
                {
                    logger.Warning("CreateChat called with null MessageContent by user {SenderId}", Context.UserIdentifier);
                    return;
                }

                var senderId = Context.UserIdentifier;
                await mediator.Send(new CreateMessageCommand(message, senderId));
                GiveUserChatMessagesDTO mess = new GiveUserChatMessagesDTO
                {
                    Content = message.MessageContent,
                    SenderId = senderId,
                    Date = DateTime.UtcNow,
                    ChatId = message.ChatId
                };

                var result = await mediator.Send(new GetChatParticipantsQuery(message.ChatId, mess.SenderId));

                var creatorId = Context.UserIdentifier;
                var creatorConnectionId = Context.ConnectionId;
                var invitedUserConnectionId = userConnections.GetValueOrDefault(result.Id);

                await Groups.AddToGroupAsync(creatorConnectionId, message.ChatId.ToString());
                logger.Information("Chat {ChatId} created and senderId: {SenderId} joined the chat", message.ChatId, senderId);

                if (invitedUserConnectionId != null)
                {
                    await Groups.AddToGroupAsync(invitedUserConnectionId, message.ChatId.ToString());
                    await Clients.Client(invitedUserConnectionId).SendAsync("NotifyNewChat");
                    logger.Information("Invited user with connectionId: {InvitedUserConnectionId} added to chat {ChatId} and notified", invitedUserConnectionId, message.ChatId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating chat initiated by user {SenderId}", Context.UserIdentifier);
                throw; 
            }
        }
    }
}
