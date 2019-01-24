using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerResponseHeaderAttribute>();

            foreach (var attr in actionAttributes)
            {
                var response = operation.Responses.FirstOrDefault(x => x.Key == ((int)attr.StatusCode).ToString(CultureInfo.InvariantCulture)).Value;

                if (response != null)
                {
                    if (response.Headers == null)
                    {
                        response.Headers = new Dictionary<string, OpenApiHeader>();
                    }

                    response.Headers.Add(attr.Name, new OpenApiHeader { Description = attr.Description, Schema = new OpenApiSchema { Type = attr.Type } });
                }
            }
        }
    }
}
