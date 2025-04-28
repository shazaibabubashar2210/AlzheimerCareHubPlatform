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
        public async Task<IActionResult> CreateCheckoutSession(DonationViewModel donation)
        {
            var domain = "https://localhost:44349"; // your domain

            SessionCreateOptions options;

            if (donation.Frequency == "Monthly")
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
            else
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

            // Save the donation data to Firebase here
            await FirebaseHelper.SubmitDonation(donation);

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

