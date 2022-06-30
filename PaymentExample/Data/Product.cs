namespace PaymentExample.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? StripeId { get; set; }

        public virtual IList<Price> Prices { get; set; }
    }
}
