namespace Chats.Domain
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public DateTime Date { get; set; }
    }
}
