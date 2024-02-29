namespace Reports.Domain
{
    public class ReportedPost
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ReportedPostId { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }
    }
}
