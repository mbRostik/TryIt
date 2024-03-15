using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;
using Users.Domain.Enums;

namespace Users.Application.Contracts.DTOs
{
    public class UserProfileDTO
    {
        public string Name { get; set; } = "";

        public string NickName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Bio { get; set; } = "";

        public byte[] Photo { get; set; } = [];

        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public bool IsPrivate { get; set; } = false;

        public int FollowersCount { get; set; } = 0;

        public int FollowsCount { get; set; } = 0;

        public string SexId { get; set; } = "UnIdentify";
    }
}
