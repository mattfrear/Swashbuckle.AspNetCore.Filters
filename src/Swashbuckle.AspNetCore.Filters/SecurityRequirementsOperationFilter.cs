using System;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly SecurityRequirementsOperationFilter<AuthorizeAttribute> filter;

        /// <summary>
        /// Constructor for SecurityRequirementsOperationFilter
        /// </summary>
        /// <param name="includeUnauthorizedAndForbiddenResponses">If true (default), then 401 and 403 responses will be added to every operation</param>
        public SecurityRequirementsOperationFilter(bool includeUnauthorizedAndForbiddenResponses = true)
        {
            Func<AuthorizeAttribute, bool> condition = (a => !string.IsNullOrEmpty(a.Policy));
            Func<AuthorizeAttribute, string> selector = (a => a.Policy);
            filter = new SecurityRequirementsOperationFilter<AuthorizeAttribute>(condition, selector, includeUnauthorizedAndForbiddenResponses);
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            filter.Apply(operation, context);
        }
    }
}