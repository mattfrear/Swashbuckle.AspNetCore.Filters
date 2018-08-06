using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AppendAuthorizeToSummaryOperationFilter<T> : IOperationFilter where T : Attribute
    {
        private readonly IEnumerable<PolicySelectorWithLabel<T>> policySelectors;

        /// <summary>
        /// Constructor for AppendAuthorizeToSummaryOperationFilter
        /// </summary>
        /// <param name="policySelectionCondition">Selects which attributes have policies. e.g. (a => !string.IsNullOrEmpty(a.Policy))</param>
        /// <param name="policySelector">Used to select the authorization policy from the attribute e.g. (a => a.Policy)</param>
        public AppendAuthorizeToSummaryOperationFilter(IEnumerable<PolicySelectorWithLabel<T>> policySelectors)
        {
            this.policySelectors = policySelectors;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var authorizeAttributes = context.GetControllerAndActionAttributes<T>();

            if (authorizeAttributes.Any())
            {
                var authorizationDescription = new StringBuilder(" (Auth");

                foreach (var policySelector in policySelectors)
                {
                    AppendPolicies(authorizeAttributes, authorizationDescription, policySelector);
                }

                operation.Summary += authorizationDescription.ToString().TrimEnd(';') + ")";
            }
        }

        private void AppendPolicies(IEnumerable<T> authorizeAttributes, StringBuilder authorizationDescription, PolicySelectorWithLabel<T> policySelector)
        {
            var policies = policySelector.Selector(authorizeAttributes)
                .OrderBy(policy => policy);

            if (policies.Any())
            {
                authorizationDescription.Append($" {policySelector.Label}: {string.Join(", ", policies)};");
            }
        }
    }
}
