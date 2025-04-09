using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;

public class PaymentController : BaseApiController
{
    private readonly IPayment _payment;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPayment payment, ILogger<PaymentController> logger)
    {
        _payment = payment;
        _logger = logger;
    }

    [HttpPost("{BasketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePayment(string BasketId)
    {
        if (string.IsNullOrWhiteSpace(BasketId))
        {
            _logger.LogWarning("Invalid BasketId provided.");
            return BadRequest("BasketId is required.");
        }

        var basket = await _payment.CreateOrUpdatePayment(BasketId);

        if (basket == null)
        {
            _logger.LogWarning("Failed to create or update payment for BasketId: {BasketId}", BasketId);
            return BadRequest("Problem creating or updating the payment.");
        }

        return Ok(basket);
    }
}