using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ShoesOnContainers.Web.WebMvc.Infrastructure
{
    public class TransferTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TransferTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IHttpClient httpClient)
        {
            httpClient.Token = await httpContext.GetTokenAsync("access_token");
            await _next.Invoke(httpContext);
        }
    }

    public static class TransferMiddleWareExtenstion
    {
        public static IApplicationBuilder UseTransferToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TransferTokenMiddleware>();
        }
    }
}
