using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ShoesOnContainers.Web.WebMvc.Infrastructure
{
    public class TokenForwardingMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenForwardingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IHttpClient httpClient)
        {
            var token = await httpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken);
            httpClient.SetBearerToken(token);
            await _next.Invoke(httpContext);
        }
    }

    public static class TokenForwardingMiddlewareExtenstion
    {
        public static IApplicationBuilder UseTokenForwarding(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenForwardingMiddleware>();
        }
    }
}
