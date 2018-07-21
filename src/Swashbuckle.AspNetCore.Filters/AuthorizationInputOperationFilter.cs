using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [Obsolete("Use SecurityRequirementsOperationFilter instead")]
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
