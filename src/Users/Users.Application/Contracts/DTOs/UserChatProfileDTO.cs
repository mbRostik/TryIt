using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Contracts.DTOs
{
    public class UserChatProfileDTO
    {
        public string Id { get; set; } = "";
        public string NickName { get; set; } = "";
        public byte[] Photo { get; set; } = [];
    }
}
