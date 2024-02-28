namespace Chats.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        public int LastMessageId { get; set; }

        public DateTime LastActivity { get; set; }

        public virtual Message LastMessage { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    }
}
