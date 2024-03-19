using Chats.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Queries
{
    public record GetChatMessagesQuery(int ChatId, string UserId) : IRequest<IEnumerable<Message>>;
}
