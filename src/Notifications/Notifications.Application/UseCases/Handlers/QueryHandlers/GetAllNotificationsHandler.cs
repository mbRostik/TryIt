using MediatR;
using Notifications.Application.UseCases.Queries;
using Notifications.Domain.Entities;
using Notifications.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Application.UseCases.Handlers.QueryHandlers
{
    public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<Notification>>
    {

        private readonly NotificationDbContext dbContext;

        public GetAllNotificationsHandler(NotificationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        {
            var result = dbContext.Notifications.ToList();
            return result;
        }


    }
}
