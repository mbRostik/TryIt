using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.UserService
{
    public class UserBannedEvent: IntegrationBaseEvent
    {
        public string UserId { get; set; }
    }
}
