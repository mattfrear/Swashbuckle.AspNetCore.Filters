using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AddHeaderOperationFilter : IOperationFilter
    {
        private readonly string parameterName;
        private readonly string description;
        private readonly bool required;

        public AddHeaderOperationFilter(string parameterName, string description, bool required = false)
        {
            this.parameterName = parameterName;
            this.description = description;
            this.required = required;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = parameterName,
                In = ParameterLocation.Header,
                Description = description,
                Required = required,
                // Type = "string"
            });
        }
    }
}
