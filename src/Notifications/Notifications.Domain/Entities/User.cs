namespace Notifications.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
