using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Application.Contracts.DTOs
{
    public class GiveFollowedPostsDTO
    {
        public string NickName { get; set; }

        public string UserId { get; set; }

        public byte[] Photo { get; set; } = [];
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Date {  get; set; }
        public List<PostFile> PostFiles { get; set; } = new List<PostFile>();

    }
}
