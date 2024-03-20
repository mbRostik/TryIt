using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Application.Contracts.DTOs
{
    public class GiveUserChatsDTO
    {
        public int ChatId { get; set; }

        public string ContactId { get; set; }

        public DateTime? lastActivity { get; set; }

        public string lastMessage { get; set; } = "";

        public string lastMessageSender { get; set; } = "";

        public string ContactNickName { get; set; }

        public byte[] ContactPhoto { get; set; }
    }
}
