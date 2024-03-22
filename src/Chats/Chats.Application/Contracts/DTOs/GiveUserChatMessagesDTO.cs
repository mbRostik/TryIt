using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Contracts.DTOs
{
    public class GiveUserChatMessagesDTO
    {
        public string SenderId { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
