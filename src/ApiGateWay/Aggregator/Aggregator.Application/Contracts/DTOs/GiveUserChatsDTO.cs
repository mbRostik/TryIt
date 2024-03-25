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

        public DateTime? LastActivity { get; set; }

        public string LastMessage { get; set; } = "";

        public string LastMessageSender { get; set; } = "";

        public string ContactNickName { get; set; }

        public byte[] ContactPhoto { get; set; }
    }
}
