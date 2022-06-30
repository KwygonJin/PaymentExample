using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentExample.Data
{
    public class Price
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string? StripeId { get; set; }
        public string Currency { get; set; }
        public int UnitAmount { get; set; }
        public string? LookupKey { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
