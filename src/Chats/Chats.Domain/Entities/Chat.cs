using System.ComponentModel.DataAnnotations.Schema;

namespace Chats.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        [NotMapped]
        public virtual Message LastMessage
        {
            get
            {
                if (Messages == null || !Messages.Any())
                    return null; 

                return Messages.OrderByDescending(m => m.Date).FirstOrDefault();
            }
        }
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    }
}
