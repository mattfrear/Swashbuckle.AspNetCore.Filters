using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi;

namespace Swashbuckle.AspNetCore.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly SecurityRequirementsOperationFilter<AuthorizeAttribute> filter;

        /// <summary>
        /// Constructor for SecurityRequirementsOperationFilter
        /// </summary>
        /// <param name="includeUnauthorizedAndForbiddenResponses">If true (default), then 401 and 403 responses will be added to every operation</param>
        /// <param name="securitySchemaName">Name of the security schema. Default value is <c>"oauth2"</c></param>
        public SecurityRequirementsOperationFilter(bool includeUnauthorizedAndForbiddenResponses = true, string securitySchemaName = "oauth2")
        {
            Func<IEnumerable<AuthorizeAttribute>, IEnumerable<string>> policySelector = authAttributes =>
                authAttributes
                    .Where(a => !string.IsNullOrEmpty(a.Policy))
                    .Select(a => a.Policy);

            filter = new SecurityRequirementsOperationFilter<AuthorizeAttribute>(policySelector, includeUnauthorizedAndForbiddenResponses, securitySchemaName);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            filter.Apply(operation, context);
        }
    }
}