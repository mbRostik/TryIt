namespace Reports.Domain
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<ReportedPost> ReportedPosts { get; set; }

        public virtual ICollection<ReportedUser> Users { get; set; }
        public virtual ICollection<ReportedUser> RepUsers { get; set; }
    }
}
