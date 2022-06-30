using Microsoft.AspNetCore.Mvc;
using PaymentExample.Models;

namespace PaymentExample.Interfaces
{
    public interface IPaymentService
    {
        public Task<CheckoutViewModel> CreateCheckoutAsync(string email);

        public Task<InvoiceViewModel> CreateInvoiceAsync(string email);

        public Task<CheckoutViewModel> CreateCheckoutSubscriptionAsync(string email, string lookupKey);

        public Task CheckPaymentStatusAsync(string sessionId);
    }
}
