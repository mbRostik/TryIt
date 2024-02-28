namespace Subscriptions.Domain
{
    public class Transaction
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public float Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; }

        public virtual User User { get; set; }
    }
}
