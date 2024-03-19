using Chats.Application.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Commands
{
    public record CreateMessageCommand(SendMessageDTO message, string SenderId) : IRequest<int>;
}
