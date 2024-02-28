namespace Subscriptions.Domain
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
