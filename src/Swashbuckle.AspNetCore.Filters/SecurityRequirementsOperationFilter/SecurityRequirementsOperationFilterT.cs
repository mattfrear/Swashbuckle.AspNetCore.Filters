using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters
{
    public class SecurityRequirementsOperationFilter<T> : IOperationFilter where T : Attribute
    {
        // inspired by https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/ResourceServer/Swagger/SecurityRequirementsOperationFilter.cs

        private readonly bool includeUnauthorizedAndForbiddenResponses;
        private readonly string securitySchemaName;
        private readonly Func<IEnumerable<T>, IEnumerable<string>> policySelector;

        /// <summary>
        /// Constructor for SecurityRequirementsOperationFilter
        /// </summary>
        /// <param name="policySelector">Used to select the authorization policy from the attribute e.g. (a => a.Policy)</param>
        /// <param name="includeUnauthorizedAndForbiddenResponses">If true (default), then 401 and 403 responses will be added to every operation</param>
        /// <param name="securitySchemaName">Name of the security schema. Default value is <c>"oauth2"</c></param>
        public SecurityRequirementsOperationFilter(
            Func<IEnumerable<T>, IEnumerable<string>> policySelector,
            bool includeUnauthorizedAndForbiddenResponses = true,
            string securitySchemaName = "oauth2")
        {
            this.policySelector = policySelector;
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this.securitySchemaName = securitySchemaName;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
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
                if (!operation.Responses.ContainsKey("401"))
                {
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                }

                if (!operation.Responses.ContainsKey("403"))
                {
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                }
            }

            var policies = policySelector(actionAttributes) ?? Enumerable.Empty<string>();

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = securitySchemaName } }, policies.ToList() }
            });
        }
    }
}