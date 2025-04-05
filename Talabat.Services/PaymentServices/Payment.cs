using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;

public class Payment : IPayment
{
    private readonly IConfiguration _configuration;
    private readonly IBasketCustomer _basketCustomer;
    private readonly ILogger<Payment> _logger;

    public Payment(IConfiguration configuration, IBasketCustomer basketCustomer, ILogger<Payment> logger)
    {
        _configuration = configuration;
        _basketCustomer = basketCustomer;
        _logger = logger;
    }

    public async Task<CustomerBasket?> CreateOrUpdatePayment(string BasketId)
    {
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        // Get the basket
        var basket = await _basketCustomer.GetBasketAsync(BasketId);

        if (basket is null)
        {
            _logger.LogWarning("Basket not found for BasketId: {BasketId}", BasketId);
            return null;
        }

        
        decimal TotalPrice = 0;

        if (basket.Items?.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                TotalPrice += item.Quantity * item.Price;
            }
        }

        
        long amountInCents = (long)(TotalPrice * 100);

        
        var service = new PaymentIntentService();
        PaymentIntent paymentIntent;

        try
        {
            if (string.IsNullOrEmpty(basket.PaymentIntentId) || !IsValidPaymentIntentId(basket.PaymentIntentId))
            {
                // Create a new payment intent
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = amountInCents,
                    Currency = _configuration["Stripe:Currency"] ?? "USD", 
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await service.CreateAsync(options);

                // Update basket with payment intent details
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update existing payment intent
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = amountInCents
                };

                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);

                // Update basket with payment intent details
                basket.PaymentIntentId = paymentIntent?.Id;
                basket.ClientSecret = paymentIntent?.ClientSecret;
            }

            // Update the basket
            await _basketCustomer.UpdateBasketAsync(basket);

            return basket;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe payment error occurred for BasketId: {BasketId}", BasketId);
            throw new ApplicationException("Stripe payment error occurred.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the payment for BasketId: {BasketId}", BasketId);
            throw new ApplicationException("An error occurred while processing the payment.", ex);
        }
    }

    private bool IsValidPaymentIntentId(string paymentIntentId)
    {
        // Stripe PaymentIntent IDs start with "pi_" followed by alphanumeric characters
        return !string.IsNullOrEmpty(paymentIntentId) &&
               paymentIntentId.StartsWith("pi_") &&
               paymentIntentId.Length > 3;
    }
}