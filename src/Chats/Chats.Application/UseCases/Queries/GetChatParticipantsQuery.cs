using Chats.Application.Contracts.DTOs;
using Chats.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Queries
{
    public record GetChatParticipantsQuery(int? id, string SenderId) : IRequest<User>;
}
