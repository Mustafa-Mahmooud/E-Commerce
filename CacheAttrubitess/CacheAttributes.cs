using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.ServiceContract;

namespace Talabat.api.Attributes
{
    public class CacheAttributes : Attribute, IAsyncActionFilter
    {
        private ICache _cacheService;

        public double Time { get; }

        public CacheAttributes(int time)
        {
            Time = time;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICache>();


            var Key = GenerateKey(context.HttpContext.Request);
            var response = await _cacheService.GetFromCacheAsync(Key);

            if (!string.IsNullOrEmpty(response))
            {
                var OkResponse = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = OkResponse;
                return;
            }

            var ExcutedResponse = await next.Invoke();

            if (ExcutedResponse.Result is OkObjectResult result)
            {
                await _cacheService.CreateCache(Key, result.Value, TimeSpan.FromSeconds(Time));

            }
        }

        public string GenerateKey(HttpRequest request)
        {
            StringBuilder CacheKey = new StringBuilder();
            CacheKey.Append(request.Path);

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                CacheKey.Append($"|{key}:{value}");
            }
            return CacheKey.ToString();
        }
    }
}