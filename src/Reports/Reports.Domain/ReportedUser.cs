namespace Reports.Domain
{
    public class ReportedUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string ReportedUserId {  get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public virtual User RepUser { get; set; }

        public virtual User User { get; set; }
    }
}
