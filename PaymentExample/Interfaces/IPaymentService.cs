using Microsoft.AspNetCore.Mvc;
using PaymentExample.Models;

namespace PaymentExample.Interfaces
{
    public interface IPaymentService
    {
        public Task<CheckoutViewModel> CreateCheckoutAsync();

        public Task<InvoiceViewModel> CreateInvoiceAsync();
    }
}
