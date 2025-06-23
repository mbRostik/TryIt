using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.Contracts.DTOs
{
    public class GiveProfilePostsDTO
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string Title { get; set; }

        public bool? PostReaction { get; set; }
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public List<string> Tags { get; set; }

        public List<GiveFileDTO> Files { get; set; }

    }
}
