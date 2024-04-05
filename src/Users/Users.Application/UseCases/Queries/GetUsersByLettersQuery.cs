using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Domain.Entities;

namespace Users.Application.UseCases.Queries
{
    public record GetUsersByLettersQuery(string SearchingField, string userId) : IRequest<List<GiveSmbProfileDTO>>;

}
