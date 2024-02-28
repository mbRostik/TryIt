namespace Users.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public virtual ICollection<SavedPost> SavedPosts { get; set; }
    }
}
