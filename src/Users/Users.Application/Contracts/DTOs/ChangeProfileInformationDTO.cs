using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Contracts.DTOs
{
    public class ChangeProfileInformationDTO
    {

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public string NickName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Bio { get; set; } = "";

        public byte[] Photo { get; set; } = [];

        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public bool IsPrivate { get; set; } = false;
    }
}
