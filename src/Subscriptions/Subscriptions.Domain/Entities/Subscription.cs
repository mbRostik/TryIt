namespace Subscriptions.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public virtual User User { get; set; }
    }
}
