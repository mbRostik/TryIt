using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        public DateTime LastActivity { get; set; }

        [NotMapped]
        public virtual Message LastMessage => Messages.OrderByDescending(m => m.Date).FirstOrDefault();
        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    }
}
