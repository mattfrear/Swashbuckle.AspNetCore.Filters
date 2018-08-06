using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public class SecurityRequirementsOperationFilter<T> : IOperationFilter where T : Attribute
    {
        // inspired by https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/ResourceServer/Swagger/SecurityRequirementsOperationFilter.cs

        private readonly bool includeUnauthorizedAndForbiddenResponses;
        private readonly Func<T, bool> policySelectionCondition;
        private readonly Func<T, string> policySelector;

        /// <summary>
        /// Constructor for SecurityRequirementsOperationFilter
        /// </summary>
        /// <param name="policySelectionCondition">Selects which attributes have policies. e.g. (a => !string.IsNullOrEmpty(a.Policy))</param>
        /// <param name="policySelector">Used to select the authorization policy from the attribute e.g. (a => a.Policy)</param>
        /// <param name="includeUnauthorizedAndForbiddenResponses">If true (default), then 401 and 403 responses will be added to every operation</param>
        public SecurityRequirementsOperationFilter(
            Func<T, bool> policySelectionCondition,
            Func<T, string> policySelector,
            bool includeUnauthorizedAndForbiddenResponses = true)
        {
            this.policySelectionCondition = policySelectionCondition;
            this.policySelector = policySelector;
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var actionAttributes = context.GetControllerAndActionAttributes<T>();

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
                .Where(policySelectionCondition)
                .Select(policySelector)
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