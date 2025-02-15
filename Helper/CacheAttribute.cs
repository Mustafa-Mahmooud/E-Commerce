using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Talabat.Core.Repositories_Interfaces;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _durationInSeconds;

    public CacheAttribute(int durationInSeconds)
    {
        _durationInSeconds = durationInSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICache>();

        var cacheKey = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;


        var cachedResponse = await cacheService.GetCachedAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
           
            context.Result = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            return;
        }

        
        var executedContext = await next();


        if (executedContext.Result is ObjectResult result && result.StatusCode == 200)
        {
            await cacheService.SetCacheAsync(cacheKey, result.Value, TimeSpan.FromSeconds(_durationInSeconds));
        }
    }
}