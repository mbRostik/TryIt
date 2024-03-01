namespace Posts.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<PFile> Files { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<PostReaction> PostReactions { get; set; }

        public virtual Ban Ban { get; set; }
    }
}
