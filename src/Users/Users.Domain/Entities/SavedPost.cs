namespace Users.Domain.Entities
{
    public class SavedPost
    {
        public string UserId { get; set; }

        public int PostId { get; set; }

        public virtual User User { get; set; }

        public virtual Post Post { get; set; }
    }
}