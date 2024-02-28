namespace Chats.Domain.Entities
{
    public class CFile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public virtual ICollection<MessageWithFile> MessageWithFiles { get; set; }
    }
}
