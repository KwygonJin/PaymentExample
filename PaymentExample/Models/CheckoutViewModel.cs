namespace PaymentExample.Models
{
    public class CheckoutViewModel
    {
        public bool IsSuccess { get; set; }
        public string? Url { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public ErrorViewModel ErrorViewModel { get; set; }
    }
}
