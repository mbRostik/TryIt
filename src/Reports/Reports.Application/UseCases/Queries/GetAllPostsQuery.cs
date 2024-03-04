using MediatR;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Queries
{
    public record GetAllPostsQuery() : IRequest<IEnumerable<Post>>;
}
