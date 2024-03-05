using MediatR;
using Subscriptions.Application.UseCases.Queries;
using Subscriptions.Domain.Entities;
using Subscriptions.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Application.UseCases.Handlers.QueryHandlers
{
    public class GetAllSubscriptionsHandler : IRequestHandler<GetAllSubscriptionsQuery, IEnumerable<Subscription>>
    {

        private readonly SubscriptionDbContext dbContext;

        public GetAllSubscriptionsHandler(SubscriptionDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var result = dbContext.Subscriptions.ToList();
            return result;
        }


    }
}