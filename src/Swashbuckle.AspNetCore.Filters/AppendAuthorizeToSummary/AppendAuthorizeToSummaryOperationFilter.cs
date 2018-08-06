using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AppendAuthorizeToSummaryOperationFilter : IOperationFilter
    {
        private readonly AppendAuthorizeToSummaryOperationFilter<AuthorizeAttribute> filter;

        public AppendAuthorizeToSummaryOperationFilter()
        {
            var policySelector = new PolicySelectorWithLabel<AuthorizeAttribute>
            {
                Label = "policies",
                Selector = authAttributes =>
                    authAttributes
                        .Where(a => !string.IsNullOrEmpty(a.Policy))
                        .Select(a => a.Policy)
            };

            var rolesSelector = new PolicySelectorWithLabel<AuthorizeAttribute>
            {
                Label = "roles",
                Selector = authAttributes =>
                    authAttributes
                        .Where(a => !string.IsNullOrEmpty(a.Roles))
                        .Select(a => a.Roles)
            };

            filter = new AppendAuthorizeToSummaryOperationFilter<AuthorizeAttribute>(new[] { policySelector, rolesSelector }.AsEnumerable());
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            filter.Apply(operation, context);
        }
    }
}
