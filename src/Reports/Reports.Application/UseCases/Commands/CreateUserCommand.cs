using MediatR;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Commands
{
    public record CreateUserCommand(User model) : IRequest<User>;
}
