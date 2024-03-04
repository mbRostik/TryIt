using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.IdentityServerService
{
    public class IdentityUserCreatedEvent: IntegrationBaseEvent
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }
    }
}
