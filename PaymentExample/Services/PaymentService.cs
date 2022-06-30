using Microsoft.AspNetCore.Mvc;
using PaymentExample.Interfaces;
using PaymentExample.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentExample.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<CheckoutViewModel> CreateCheckoutAsync()
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
                CancelUrl = "https://localhost:7180/Payment/Cancel",
            };

            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
            var service = new SessionService();
            try
            {
                Session session = await service.CreateAsync(options);
                checkoutViewModel.IsSuccess = true;
                checkoutViewModel.Url = session.Url;
            }
            catch (StripeException ex)
            {
                checkoutViewModel.IsSuccess = false;
                checkoutViewModel.ErrorViewModel = new ErrorViewModel
                {
                    ErrorText = GetErrorTextFromStripeError(ex)
                };
            }

            return checkoutViewModel;
        }

        public async Task<InvoiceViewModel> CreateInvoiceAsync()
        {
            string email = "Kalyan3107@gmail.com";
            string collectionMethod = "send_invoice";
            string productName = "test_product";
            string priceName = "tesе_price";
            int daysToDie = 30;
            int unitAmount = 100;

            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

            try
            {
                var customer = await CreaceStripeCustomerAsync(email);
                var stripeProduct = await CreaceStripeProductAsync(productName);
                var price = await CreaceStripePriceAsync(priceName, stripeProduct.Id, unitAmount);

                var invoiceItemOptions = new InvoiceItemCreateOptions
                {
                    Customer = customer.Id,
                    Price = price.Id
                };

                var invoiceItemService = new InvoiceItemService();
                await invoiceItemService.CreateAsync(invoiceItemOptions);

                // Create an Invoice
                var invoiceOptions = new InvoiceCreateOptions
                {
                    Customer = customer.Id,
                    CollectionMethod = collectionMethod,
                    DaysUntilDue = daysToDie,
                };
                var invoiceService = new InvoiceService();
                var invoice = await invoiceService.CreateAsync(invoiceOptions);

                await invoiceService.SendInvoiceAsync(invoice.Id);

                invoiceViewModel.IsSuccess = true;
            }
            catch (StripeException ex)
            {
                invoiceViewModel.IsSuccess = false;
                invoiceViewModel.ErrorViewModel = new ErrorViewModel
                {
                    ErrorText = GetErrorTextFromStripeError(ex)
                };
            }

            return invoiceViewModel;
        }

        private async Task<Customer> CreaceStripeCustomerAsync(string email, string description = "Test customer to invoice")
        {
            var customerService = new CustomerService();
            var customerOptions = new CustomerCreateOptions
            {
                Email = email,
                Description = description
            };
            var stripeCustomer = await customerService.CreateAsync(customerOptions);

            return stripeCustomer;
        }

        private async Task<Product>  CreaceStripeProductAsync(string productName)
        {
            var productOptions = new ProductCreateOptions
            {
                Name = productName
            };
            var productService = new ProductService();
            var stripeProduct = await productService.CreateAsync(productOptions);

            return stripeProduct;
        }

        private async Task<Price> CreaceStripePriceAsync(string priceName, string stripeProductId, int unitAmount)
        {
            var priceOptions = new PriceCreateOptions
            {
                Currency = "usd",
                Nickname = priceName,
                Product = stripeProductId,
                UnitAmount = unitAmount
            };
            var priceService = new PriceService();
            var stripePrice = await priceService.CreateAsync(priceOptions);

            return stripePrice;
        }

        private static string GetErrorTextFromStripeError(StripeException ex)
        {
            string errorText = "";
            if (ex.StripeError.Type == "card_error")
            {
                if (ex.StripeError.PaymentIntent.Charges.Data[0].Outcome.Type == "blocked")
                {
                    errorText = "Payment blocked for suspected fraud.";
                }
                else if (ex.StripeError.Code == "card_declined")
                {
                    errorText = "Declined by the issuer.";
                }
                else if (ex.StripeError.Code == "expired_card")
                {
                    errorText = "Card expired.";
                }
                else
                {
                    errorText = $"A payment error occurred: {ex.StripeError.Message}";
                }
            }
            else if (ex.StripeError.Type == "invalid_request_error"){
                errorText = $"An invalid request occurred: {ex.StripeError.Message}";
            }
            else
            {
                errorText = $"Another problem occurred, maybe unrelated to Stripe. Error: {ex.StripeError.Message}";
            }

            return errorText;
        }
    }
}
