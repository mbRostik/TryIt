namespace Posts.Domain.Entities
{
    public class PFile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] file { get; set; }

        public DateTime Date { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }
    }
}
