namespace Reports.Domain
{
    public class Post
    {
        public int Id { get; set; } 
        public virtual ICollection<ReportedPost> ReportedPosts { get; set; }
    }
}
