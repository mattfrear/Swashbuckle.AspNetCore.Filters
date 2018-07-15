using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [Obsolete("Use SecurityRequirementsOperationFilter from here instead: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/ResourceServer/Swagger/SecurityRequirementsOperationFilter.cs")]
    public class AuthorizationInputOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Description = "access token",
                Required = false,
                Type = "string"
            });
        }
    }
}
