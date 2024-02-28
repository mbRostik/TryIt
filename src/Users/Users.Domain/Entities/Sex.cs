using Users.Domain.Enums;

namespace Users.Domain.Entities
{
    public class Sex
    {
        public int Id { get; set; }
        public SexType SexType { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
