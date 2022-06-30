using Microsoft.AspNetCore.Mvc;
using PaymentExample.Interfaces;
using PaymentExample.Interfaces.IRepository;
using PaymentExample.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentExample.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private string currencyDefault = "usd";
        private string collectionMethodCheckoutPayment = "payment";
        private string collectionMethodCheckoutSubscription = "subscription";
        private string collectionMethodInvoice = "send_invoice";
        private string statusPaid = "paid";
        private int productIdDefault = 3;
        private string priceNameDefault = "tesе_price";
        private int daysToDieDefault = 30;
        private int quantityDefault = 1;
        private int unitAmountDefault = 100;
        private string domain = "https://localhost:7180/";

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckoutViewModel> CreateCheckoutAsync(string email)
        {
            var customer = await _unitOfWork.Cusotmers.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                customer = new PaymentExample.Data.Customer
                {
                    Name = email,
                    Email = email,
                    Description = email
                };
                await _unitOfWork.Cusotmers.AddCustomerAsync(customer);
                await _unitOfWork.SaveAsync();
            }

            if (customer.StripeId == null)
            {
                var stripeCustomer = await CreateStripeCustomerAsync(customer.Email);
                customer.StripeId = stripeCustomer.Id;
                _unitOfWork.Cusotmers.UpdateCustomer(customer);
                await _unitOfWork.SaveAsync();
            }
            var product = await _unitOfWork.Products.GetProductAsync(productIdDefault);

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = unitAmountDefault,
                      Currency = currencyDefault,
                      Product = product.StripeId
                    },
                    Quantity = quantityDefault,
                  },
                },
                Mode = this.collectionMethodCheckoutPayment,
                Customer = customer.StripeId,
                SuccessUrl = domain + "Payment/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "Payment/Cancel",
            };

            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
            var service = new SessionService();
            try
            {
                Session session = await service.CreateAsync(options);
                checkoutViewModel.IsSuccess = true;
                checkoutViewModel.Url = session.Url;
                checkoutViewModel.SessionId = session.Id;
                checkoutViewModel.PaymentIntentId = session.PaymentIntentId;

                PaymentExample.Data.Payment payment = await _unitOfWork.Payments.AddPaymentAsync(new Data.Payment { 
                    CustomertId = customer.Id,
                    SessionId = session.Id,
                    PaymentId = session.PaymentIntentId,
                    Description = product.Name
                });
                await _unitOfWork.SaveAsync();
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

        public async Task<InvoiceViewModel> CreateInvoiceAsync(string email)
        {
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

            try
            {
                var customer = await CreateStripeCustomerAsync(email);
                var stripeProduct = await CreateStripeProductAsync(priceNameDefault);
                var price = await CreateStripePriceAsync(priceNameDefault, stripeProduct.Id, unitAmountDefault);

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
                    CollectionMethod = collectionMethodInvoice,
                    DaysUntilDue = daysToDieDefault,
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

        public async Task<CheckoutViewModel> CreateCheckoutSubscriptionAsync(string email, string lookupKey)
        {
            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();

            try
            {
                var customer = await _unitOfWork.Cusotmers.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    customer = new PaymentExample.Data.Customer
                    {
                        Name = email,
                        Email = email,
                        Description = email
                    };
                    await _unitOfWork.Cusotmers.AddCustomerAsync(customer);
                    await _unitOfWork.SaveAsync();
                }

                if (customer.StripeId == null)
                {
                    var stripeCustomer = await CreateStripeCustomerAsync(customer.Email);
                    customer.StripeId = stripeCustomer.Id;
                    _unitOfWork.Cusotmers.UpdateCustomer(customer);
                    await _unitOfWork.SaveAsync();
                }

                var price = await _unitOfWork.Prices.GetPriceByLookUpAsync(lookupKey);

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = price.StripeId,
                            Quantity = quantityDefault,
                        },
                    },
                    Customer = customer.StripeId,
                    Mode = collectionMethodCheckoutSubscription,
                    SuccessUrl = domain + "Payment/SuccessSubscription?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "Payment/Cancel",
                };
                var service = new SessionService();
                Session session = await service.CreateAsync(options);
                checkoutViewModel.IsSuccess = true;
                checkoutViewModel.Url = session.Url;
                checkoutViewModel.SessionId = session.Id;
                checkoutViewModel.PaymentIntentId = session.SubscriptionId;

                PaymentExample.Data.Payment payment = await _unitOfWork.Payments.AddPaymentAsync(new Data.Payment
                {
                    CustomertId = customer.Id,
                    SessionId = session.Id,
                    PaymentId = session.PaymentIntentId,
                    Description = price.Nickname
                });
                await _unitOfWork.SaveAsync();
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

        public async Task<Customer> CreateStripeCustomerAsync(string email, string description = "Test customer to invoice")
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

        public async Task<Product>  CreateStripeProductAsync(string productName)
        {
            var productOptions = new ProductCreateOptions
            {
                Name = productName
            };
            var productService = new ProductService();
            var stripeProduct = await productService.CreateAsync(productOptions);

            return stripeProduct;
        }

        public async Task<Price> CreateStripePriceAsync(string priceName, string stripeProductId, int unitAmount)
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

        public async Task CheckPaymentStatusAsync(string sessionId)
        {
            var service = new SessionService();
            Session session = await service.GetAsync(sessionId);
            if (session.PaymentStatus.ToLower() == statusPaid)
            {
                PaymentExample.Data.Payment payment = await _unitOfWork.Payments.GetPaymentBySessionAsync(sessionId);
                if (payment != null)
                {
                    payment.IsSuccess = true;
                    _unitOfWork.Payments.UpdatePayment(payment);
                    await _unitOfWork.SaveAsync();
                }
            }
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
            else if (ex.StripeError.Type == "invalid_request_error")
            {
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
