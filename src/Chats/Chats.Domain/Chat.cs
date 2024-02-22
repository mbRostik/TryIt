namespace Chats.Domain
{
    public class Chat
    {
        public int Id { get; set; }

        public string FirstUserId { get; set; }

        public string SecondUserId { get; set; }

        public int LastMessageId { get; set; }

        public DateTime LastActivity { get; set; }
    }
}
