using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPosts.Models
{
    public class PostReaction
    {

        public string UserId { get; set; }

        public int PostId { get; set; }

        public bool Reaction { get; set; } = true;

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
