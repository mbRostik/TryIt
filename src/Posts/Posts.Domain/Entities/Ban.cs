namespace Posts.Domain.Entities
{
    public class Ban
    {
        public int Id { get; set; }

        public int BannedPostId { get; set; }

        public string ModeratorId { get; set; }

        public string BanReason { get; set; }

        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public virtual Post Post { get; set; }
    }
}
