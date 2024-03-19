using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Contracts.DTOs
{
    public class SendMessageDTO
    {
        public string ReceiverId { get; set; }
        public string? MessageContent { get; set; }
        public byte[]? Data { get; set; }

        public int? ChatId { get; set; }
    }
}
