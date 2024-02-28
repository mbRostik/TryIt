namespace Users.Domain.Entities
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

        public virtual ICollection<SavedPost> SavedPosts { get; set;}

        public virtual ICollection<Follow> Follows { get; set; }

        public virtual ICollection<Follow> Followers { get; set; }

        public virtual ICollection<BannedUser> BannedBy { get; set; }

        public virtual ICollection<BannedUser> BannedUsers { get; set; }

        public virtual Sex Sex { get; set; }
    }
}
