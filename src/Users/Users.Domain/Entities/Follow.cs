namespace Users.Domain.Entities
{
    public class Follow
    {
        public string UserId { get; set; }

        public string FollowerId { get; set; }

        public virtual User Follower {  get; set; }

        public virtual User User { get; set; }
    }
}
