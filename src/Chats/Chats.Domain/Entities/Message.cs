namespace Chats.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        public string SenderId { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public virtual ICollection<MessageWithFile> MessageWithFiles { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Chat> Chats { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
