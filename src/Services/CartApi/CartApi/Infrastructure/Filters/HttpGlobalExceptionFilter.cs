using System;
using System.Net;
using CartApi.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CartApi.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(
                new EventId(
                    context.Exception.HResult),
                    context.Exception,
                    context.Exception.Message);
            if (context.Exception.GetType() == typeof(CartDomainException))
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] {context.Exception.Message}
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }

            context.ExceptionHandled = true;
        }
    }

    public class JsonErrorResponse
    {
        public string[] Messages { get; set; }
    }
}
