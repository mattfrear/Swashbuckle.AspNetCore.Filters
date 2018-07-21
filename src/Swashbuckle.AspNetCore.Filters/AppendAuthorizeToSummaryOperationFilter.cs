using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AppendAuthorizeToSummaryOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var authorizeAttributes = context.GetControllerAndActionAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                var authorizationDescription = new StringBuilder(" (Auth");

                AppendPolicies(authorizeAttributes, authorizationDescription);
                AppendRoles(authorizeAttributes, authorizationDescription);

                operation.Summary += authorizationDescription.ToString().TrimEnd(';') + ")";
            }
        }

        private static void AppendPolicies(IEnumerable<AuthorizeAttribute> authorizeAttributes, StringBuilder authorizationDescription)
        {
            var policies = authorizeAttributes
                .Where(a => !string.IsNullOrEmpty(a.Policy))
                .Select(a => a.Policy)
                .OrderBy(policy => policy);

            if (policies.Any())
            {
                authorizationDescription.Append($" policies: {string.Join(", ", policies)};");
            }
        }

        private static void AppendRoles(IEnumerable<AuthorizeAttribute> authorizeAttributes, StringBuilder authorizationDescription)
        {
            var roles = authorizeAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .Select(a => a.Roles)
                .OrderBy(role => role);

            if (roles.Any())
            {
                authorizationDescription.Append($" roles: {string.Join(", ", roles)};");
            }
        }
    }
}
