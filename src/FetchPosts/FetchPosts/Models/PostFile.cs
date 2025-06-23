using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPosts.Models
{
    public class PostFile
    {
        public int Id { get; set; }
        public int PostId { get; set; }

        public string Name { get; set; }

        public byte[] file { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

    }
}
