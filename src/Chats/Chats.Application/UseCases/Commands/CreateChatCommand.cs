using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Commands
{
    public record CreateChatCommand(string FirstUser, string SecondUser) : IRequest<int>;

}
