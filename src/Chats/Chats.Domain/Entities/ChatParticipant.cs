using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Domain.Entities
{
    public class ChatParticipant
    {
        public int ChatId { get; set; }

        public string UserId { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual User User { get; set; }
    }
}
