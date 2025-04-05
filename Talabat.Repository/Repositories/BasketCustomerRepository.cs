using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;

public class BasketCustomerRepository : IBasketCustomer
{
    private readonly IDatabase _redis;
    private readonly ILogger<BasketCustomerRepository> _logger;

    public BasketCustomerRepository(IConnectionMultiplexer redis, ILogger<BasketCustomerRepository> logger)
    {
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<bool> DeleteBasketAsync(string BasketId)
    {
        return await _redis.KeyDeleteAsync(BasketId);
    }

    public async Task<CustomerBasket> GetBasketAsync(string BasketId)
    {
        try
        {
            var basket = await _redis.StringGetAsync(BasketId);

            if (string.IsNullOrWhiteSpace(basket))
            {
                _logger.LogWarning("Basket not found for BasketId: {BasketId}", BasketId);
                return null;
            }

            return JsonSerializer.Deserialize<CustomerBasket>(basket);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize basket for BasketId: {BasketId}", BasketId);
            throw new ApplicationException("Failed to deserialize basket.", ex);
        }
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
    {
        try
        {
            var basketJson = JsonSerializer.Serialize(customerBasket);
            var success = await _redis.StringSetAsync(customerBasket.Id, basketJson, TimeSpan.FromDays(10));

            if (!success)
            {
                _logger.LogError("Failed to update basket for BasketId: {BasketId}", customerBasket.Id);
                return null;
            }

            return await GetBasketAsync(customerBasket.Id);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to serialize basket for BasketId: {BasketId}", customerBasket.Id);
            throw new ApplicationException("Failed to serialize basket.", ex);
        }
    }
}