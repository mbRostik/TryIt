using Notifications.Domain.Enums;

namespace Notifications.Domain.Entities
{
    public class NotificationType
    {
        public int Id { get; set; }
        public NotificationTypes NotificationTypes { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
