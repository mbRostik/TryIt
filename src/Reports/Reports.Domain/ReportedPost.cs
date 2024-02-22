namespace Reports.Domain
{
    public class ReportedPost
    {
        public int Id { get; set; }

        public string ModeratorId { get; set; }

        public string ReportedPostId { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }
    }
}
