using MediatR;
using Subscriptions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Application.UseCases.Commands
{
    public record CreateUserCommand(User model) : IRequest<User>;

}
