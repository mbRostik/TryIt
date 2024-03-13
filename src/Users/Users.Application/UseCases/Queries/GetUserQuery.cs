using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;

namespace Users.Application.UseCases.Queries
{
    public record GetUserQuery(string id) : IRequest<UserProfileDTO>;

}
