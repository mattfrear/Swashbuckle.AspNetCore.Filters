using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swashbuckle.AspNetCore.Examples
{
    public class AppendAuthorizeToSummaryOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.ApiDescription
                .ActionAttributes()
                .OfType<AuthorizeAttribute>()
                .ToList();

            authorizeAttributes.AddRange(context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>());

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
