using Microsoft.AspNetCore.Mvc;
using PaymentExample.Interfaces;
using PaymentExample.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentExample.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutAsync()
        {
            CheckoutViewModel checkoutViewModel = await _paymentService.CreateCheckoutAsync();
            if (checkoutViewModel == null)
                return View("Cancel", new ErrorViewModel
                {
                    ErrorText = "Internal error"
                });

            if (checkoutViewModel.IsSuccess)
            {
                Response.Headers.Add("Location", checkoutViewModel.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return View("Cancel", checkoutViewModel.ErrorViewModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> InvoiceAsync()
        {
            InvoiceViewModel invoiceViewModel = await _paymentService.CreateInvoiceAsync();
            if (invoiceViewModel == null)
                return View("Cancel", new ErrorViewModel
                {
                    ErrorText = "Internal error"
                });

            if (invoiceViewModel.IsSuccess)
            {
                return View("Success");
            }
            else
            {
                return View("Cancel", invoiceViewModel.ErrorViewModel);
            }
        }


        [HttpPost]
        [Route("create-checkout-session")]
        public ActionResult Create()
        {
            var domain = "https://localhost:7180";

            var priceOptions = new PriceListOptions
            {
                LookupKeys = new List<string> {
                    Request.Form["lookup_key"]
                }
            };
            var priceService = new PriceService();
            StripeList<Price> prices = priceService.List(priceOptions);

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = prices.Data[0].Id,
                        Quantity = 1,
                    },
                },
                Customer = "cus_LxvFYAhU588eDC",
                Mode = "subscription",
                SuccessUrl = domain + "/Payment/SuccessSubscription?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/Payment/Cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Route("create-portal-session")]
        [HttpPost]
        public ActionResult CreatePortalSession()
        {
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutService = new SessionService();
            var checkoutSession = checkoutService.Get(Request.Form["session_id"]);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = "https://localhost:7180/";

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = returnUrl,
            };
            var service = new Stripe.BillingPortal.SessionService();
            var session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult SuccessSubscription()
        {
            ViewBag.sessionId = Request.Query["session_id"].ToString();
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
