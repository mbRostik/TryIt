using MediatR;
using Notifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Application.UseCases.Queries
{
    public record GetAllNotificationsQuery() : IRequest<IEnumerable<Notification>>;

}
