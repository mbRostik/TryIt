namespace Notifications.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public int NotificationTypeId { get; set; }
        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public virtual NotificationType NotificationType { get; set; }
    }
}
