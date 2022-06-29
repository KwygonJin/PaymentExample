using Microsoft.AspNetCore.Mvc;
using PaymentExample.Interfaces;
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
        public async Task<IActionResult> CreateCheckout()
        {
            string url = await _paymentService.CreateCheckoutAsync();
            Response.Headers.Add("Location", url);
            return new StatusCodeResult(303);
        }

        [HttpPost]
        public IActionResult Invoice()
        {
            string invoiceId = _paymentService.CreateInvoice();
            return View("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
