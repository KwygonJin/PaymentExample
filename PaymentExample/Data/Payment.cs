using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentExample.Data
{
    public class Payment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentId { get; set; }
        public bool IsSuccess { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomertId { get; set; }
        public Customer Customer { get; set; }
    }
}
