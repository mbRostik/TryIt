using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.PostService
{
    public class PostCreatedEvent: IntegrationBaseEvent
    {
        public int PostId { get; set; }  
    }
}
