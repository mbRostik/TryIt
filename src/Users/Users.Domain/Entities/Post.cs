namespace Users.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<SavedPost> SavedPosts { get; set; }
    }
}
