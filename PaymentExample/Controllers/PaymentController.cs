using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace PaymentExample.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Processing()
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
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
