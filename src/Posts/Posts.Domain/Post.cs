
namespace Posts.Domain
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
    }
}
