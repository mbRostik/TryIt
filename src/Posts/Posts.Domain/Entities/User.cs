namespace Posts.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PostReaction> PostReactions { get; set; }
        public virtual ICollection<Ban> Bans { get; set; }
        public virtual ICollection<CommentReaction> CommentReactions { get; set; }

    }
}
