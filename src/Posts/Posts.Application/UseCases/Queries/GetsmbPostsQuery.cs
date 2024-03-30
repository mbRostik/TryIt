using MediatR;
using Posts.Application.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Queries
{
    public record GetsmbPostsQuery(string id) : IRequest<List<GiveProfilePostsDTO>>;

}
