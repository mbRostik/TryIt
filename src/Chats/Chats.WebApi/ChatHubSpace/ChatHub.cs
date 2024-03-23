using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Chats.WebApi.ChatHubSpace
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        private readonly IMediator mediator;

        public ChatHub(ChatDbContext context, IMediator mediator)
        {
            _context = context;
            this.mediator = mediator;

        }

        public async Task JoinChat(int chatId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, chatId.ToString());
        }


        public async Task SendMessage(SendMessageDTO message)
        {
            var senderId = Context.UserIdentifier;
            await mediator.Send(new CreateMessageCommand(message, senderId));
            GiveUserChatMessagesDTO mess = new GiveUserChatMessagesDTO();
            mess.Content = message.MessageContent;
            mess.SenderId = senderId;
            mess.Date = DateTime.UtcNow;
            mess.ChatId = message.ChatId;
            await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", mess);

        }
        private static readonly Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                Console.WriteLine(userId + " connected");
                userConnections[userId] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null && userConnections.ContainsKey(userId))
            {
                userConnections.Remove(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateChat(SendMessageDTO message)
        {
            
            if (message.MessageContent == null)
            {
                return;
            }

            var senderId = Context.UserIdentifier;
            await mediator.Send(new CreateMessageCommand(message, senderId));
            GiveUserChatMessagesDTO mess = new GiveUserChatMessagesDTO();
            mess.Content = message.MessageContent;
            mess.SenderId = senderId;
            mess.Date = DateTime.UtcNow;
            mess.ChatId = message.ChatId;


            var result = await mediator.Send(new GetChatParticipantsQuery(message.ChatId, mess.SenderId));

            var creatorId = Context.UserIdentifier;
            var creatorConnectionId = Context.ConnectionId;
            var invitedUserConnectionId = userConnections.GetValueOrDefault(result.Id);

            await Groups.AddToGroupAsync(creatorConnectionId, message.ChatId.ToString());

            if (invitedUserConnectionId != null)
            {

                await Groups.AddToGroupAsync(invitedUserConnectionId, message.ChatId.ToString());

                await Clients.Client(invitedUserConnectionId).SendAsync("NotifyNewChat");
            }
        }
    }
}
