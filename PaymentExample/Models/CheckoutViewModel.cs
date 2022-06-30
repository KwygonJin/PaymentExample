namespace PaymentExample.Models
{
    public class CheckoutViewModel
    {
        public bool IsSuccess { get; set; }
        public string? Url { get; set; }
        public ErrorViewModel ErrorViewModel { get; set; }
    }
}
