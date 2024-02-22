namespace User.Domain
{
    public class Follow
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FollowerId { get; set; }
    }
}
