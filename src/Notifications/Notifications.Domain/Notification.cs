namespace Notifications.Domain
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public int TypeId { get; set; }
        public DateTime Date { get; set; }
    }
}
