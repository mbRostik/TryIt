namespace Posts.Domain.Entities
{
    public class PostReaction
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int PostId { get; set; }

        public bool Reaction { get; set; }

        public DateTime Date { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}
