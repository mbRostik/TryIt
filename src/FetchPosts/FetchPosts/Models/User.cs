using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPosts.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; } = "test@gmail.com";
        public string Phone { get; set; } = "0";
        public string Bio { get; set; } = "none";
        public string Photo { get; set; } = "";
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public int SexId { get; set; } = 1;
        public bool IsBanned { get; set; } = false;
        public bool IsPrivate { get; set; } = false;
        public bool IsCheckingMessages { get; set; } = false;
    }
}
