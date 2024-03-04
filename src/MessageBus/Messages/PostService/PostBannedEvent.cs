using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.PostService
{
    public class PostBannedEvent : IntegrationBaseEvent
    {
        public int PostId { get; set; }
    }
}
