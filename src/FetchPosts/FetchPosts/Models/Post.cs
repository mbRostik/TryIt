using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPosts.Models
{
    public class Post
    {

        public string Title { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
