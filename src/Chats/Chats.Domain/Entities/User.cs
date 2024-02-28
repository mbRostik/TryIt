namespace Chats.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    }
}
