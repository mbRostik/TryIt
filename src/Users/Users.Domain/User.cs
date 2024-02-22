namespace Users.Domain
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public byte[] Photo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int SexId { get; set; }
        public bool IsBanned { get; set; }
        public bool IsPrivate { get; set; }
    }
}
