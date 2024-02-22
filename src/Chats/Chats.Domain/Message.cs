namespace Chats.Domain
{
    public class Message
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        public string SenderId { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime Date { get; set; }

    }
}
