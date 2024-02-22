namespace Reports.Domain
{
    public class ReportedUser
    {
        public int Id { get; set; }
        public string ModeratorId { get; set; }

        public string ReportedUserId {  get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }
    }
}
