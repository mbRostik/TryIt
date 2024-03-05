using MediatR;
using Subscriptions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Application.UseCases.Queries
{
    public record GetAllSubscriptionsQuery() : IRequest<IEnumerable<Subscription>>;
}
