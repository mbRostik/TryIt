using MediatR;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Queries
{
    public record GetAllPostsQuery() : IRequest<IEnumerable<Post>>;
}
