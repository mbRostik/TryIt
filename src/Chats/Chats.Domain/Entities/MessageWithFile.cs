namespace Chats.Domain.Entities
{
    public class MessageWithFile
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public int MessageId { get; set; }

        public virtual CFile File { get; set; }

        public virtual Message Message { get; set; }
    }
}
