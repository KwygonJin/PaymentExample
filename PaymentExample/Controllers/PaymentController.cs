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
            string customerEmail = Request.Form["customerEmail"];
            CheckoutViewModel checkoutViewModel = await _paymentService.CreateCheckoutAsync(customerEmail);
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
            string customerEmail = Request.Form["customerEmail"];
            InvoiceViewModel invoiceViewModel = await _paymentService.CreateInvoiceAsync(customerEmail);
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
        public async Task<IActionResult> CreateCheckoutSubscriptionAsync()
        {
            string customerEmail = Request.Form["customerEmail"];
            CheckoutViewModel checkoutViewModel = await _paymentService.CreateCheckoutSubscriptionAsync(customerEmail, Request.Form["lookup_key"]);
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

        public async Task<IActionResult> Success()
        {
            if (Request.Query["session_id"].Count > 0)
                await _paymentService.CheckPaymentStatusAsync(Request.Query["session_id"].ToString());
            return View();
        }

        public async Task<IActionResult> SuccessSubscription()
        {           
            if (Request.Query["session_id"].Count > 0)
            {
                ViewBag.sessionId = Request.Query["session_id"].ToString();
                await _paymentService.CheckPaymentStatusAsync(Request.Query["session_id"].ToString());
            }
            else
            {
                ViewBag.sessionId = "Internal error";
            }

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
