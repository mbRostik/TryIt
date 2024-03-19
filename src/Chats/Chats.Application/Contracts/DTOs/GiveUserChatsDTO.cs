using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Contracts.DTOs
{
    public class GiveUserChatsDTO
    {
        public int ChatId { get; set; }

        public string ContactId { get; set; }

        public DateTime LastActivity { get; set; }

        public string LastMessage { get; set; }
    }
}
