using Microsoft.AspNetCore.Mvc;

namespace PaymentExample.Interfaces
{
    public interface IPaymentService
    {
        public Task<string> CreateCheckoutAsync();

        public string CreateInvoice();
    }
}
