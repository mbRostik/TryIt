using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class CommentReaction
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CommentId { get; set; }

        public bool Reaction { get; set; }

        public DateTime Date { get; set; }

        public virtual Comment Comment { get; set; }

        public virtual User User { get; set; }
    }
}
