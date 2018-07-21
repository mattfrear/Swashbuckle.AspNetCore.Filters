using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        // modified from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/ResourceServer/Swagger/SecurityRequirementsOperationFilter.cs

        private readonly bool includeUnauthorizedAndForbiddenResponses;

        public SecurityRequirementsOperationFilter(bool includeUnauthorizedAndForbiddenResponses = true)
        {
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var actionAttributes = context.GetControllerAndActionAttributes<AuthorizeAttribute>();

            if (!actionAttributes.Any())
            {
                return;
            }

            if (includeUnauthorizedAndForbiddenResponses)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });
            }

            var policies = actionAttributes
                .Where(a => !string.IsNullOrEmpty(a.Policy))
                .Select(a => a.Policy)
                ?? Enumerable.Empty<string>();

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "oauth2", policies }
                    }
                };
        }
    }
}