using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleStore.Models;
using Stripe;

public class PaymentController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly StripeSettings _stripeSettings;

    public PaymentController(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment()
    {
        var cart = GetCart();
        var totalAmount = (long)(cart.TotalValue * 100); // Сумма в центах

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        var options = new PaymentIntentCreateOptions
        {
            Amount = totalAmount,
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        PaymentIntent intent = await service.CreateAsync(options);

        return Json(new { clientSecret = intent.ClientSecret });
    }

    public IActionResult Success()
    {
        return View();
    }

    private Cart GetCart()
    {
        var cart = HttpContext.Session.GetString("Cart");
        return cart == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cart);
    }
}
