using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.Contracts.DTOs
{
    public class CreatePostDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; } = "";

        public List<FileModel> Files { get; set; } = new List<FileModel>();
    }
}
