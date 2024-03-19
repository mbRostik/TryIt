using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Notifications
{
    public record ChatCreatedNotification(string FirstUser, string SecondUser, int ChatId) : INotification;

}
