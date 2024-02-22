
namespace Posts.Domain
{
    public class PostReaction
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int PostId { get; set; }

        public bool Reaction {  get; set; }

        public DateTime Date { get; set; }
    }
}
