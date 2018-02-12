using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CartApi.Infrastructure.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any()
                             | context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Description += "Abgesicherte Operation";
                operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new Response{ Description = "Unauthorized"});
                operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new Response { Description = "Forbidden" });
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"oauth2", new[] {"basket"}}
                    }
                };
            }
        }
    }
}
