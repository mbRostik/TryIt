namespace Users.Domain.Entities
{
    public class BannedUser
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string ModeratorId { get; set; }

        public string BanReason { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual User User { get; set; } 

        public virtual User BannedByUser { get; set; }
    }
}
