using AlzhCareHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace AlzhCareHub.Controllers
{
    public class DonationController : Controller
    {
        private readonly StripeSettings _stripeSettings;

        public DonationController(IOptions<StripeSettings> stripeOptions)
        {
            _stripeSettings = stripeOptions.Value;
        }

        public IActionResult Donate()
        {
            ViewBag.PublishableKey = _stripeSettings.PublishableKey;
            return View();
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession(DonationViewModel donation)
        {
            var domain = "https://localhost:44349"; // change to your domain

            SessionCreateOptions options;

            if (donation.Frequency == "Monthly")
            {
                // For recurring, create a subscription with a recurring price
                options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(donation.Amount * 100),
                        Currency = "usd",
                        Recurring = new SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = "month"
                        },
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = donation.DonationType
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "subscription",
                    SuccessUrl = domain + "/Donation/Success",
                    CancelUrl = domain + "/Donation/Cancel",
                };
            }
            else // One time donation
            {
                options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(donation.Amount * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = donation.DonationType
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = domain + "/Donation/Success",
                    CancelUrl = domain + "/Donation/Cancel",
                };
            }

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}

