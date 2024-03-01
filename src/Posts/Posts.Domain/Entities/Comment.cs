using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Replies { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<CommentReaction> CommentReactions { get; set; }
    }
}
