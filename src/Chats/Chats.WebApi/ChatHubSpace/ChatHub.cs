using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
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
            mess.Date= DateTime.UtcNow;
            mess.ChatId = message.ChatId;
            await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", mess);
            
        }

    }
}
