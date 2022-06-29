using Microsoft.AspNetCore.Mvc;
using PaymentExample.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentExample.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<string> CreateCheckoutAsync()
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = 2000,
                      Currency = "usd",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "T-shirt",
                      },

                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7180/Payment/Success",
                CancelUrl = "https://localhost:7180/Payment/",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return session.Url;
        }

        public string CreateInvoice()
        {
            string email = "Kalyan3107@gmail.com";
            string collectionMethod = "send_invoice";
            string productName = "test_product";
            string priceName = "tesе_price";
            int daysToDie = 30;

            var customerOptions = new CustomerCreateOptions
            {
                Email = email,
                Description = "Test customer to invoice",
            };
            var customerService = new CustomerService();
            var stripeCustomer = customerService.Create(customerOptions);
            string customerId = stripeCustomer.Id;

            var productOptions = new ProductCreateOptions
            {
                Name = productName
            };
            var productService = new ProductService();
            var stripeProduct = productService.Create(productOptions);

            var priceOptions = new PriceCreateOptions
            {
                Active = true,
                Currency = "USD",
                Nickname = priceName,
                Product = stripeProduct.Id,
                UnitAmount = 100
            };
            var priceService = new PriceService();
            var stripePrice = priceService.Create(priceOptions);
            string priceId = stripePrice.Id;

            var invoiceItemOptions = new InvoiceItemCreateOptions
            {
                Customer = customerId,
                Price = priceId,
            };

            var invoiceItemService = new InvoiceItemService();
            invoiceItemService.Create(invoiceItemOptions);
            // Create an Invoice
            var invoiceOptions = new InvoiceCreateOptions
            {
                Customer = customerId,
                CollectionMethod = collectionMethod,
                DaysUntilDue = daysToDie,
            };
            var invoiceService = new InvoiceService();
            var invoice = invoiceService.Create(invoiceOptions);
                
            invoiceService.SendInvoice(invoice.Id);

            return invoice.Id;
        }

        private Customer FindOrCreaceStripeCustomer(string email, string description = "Test customer to invoice")
        {
            var customerService = new CustomerService();
            var customerOptions = new CustomerCreateOptions
            {
                Email = email,
                Description = description
            };
            var stripeCustomer = customerService.Create(customerOptions);

            return stripeCustomer;
        }
    }
}
