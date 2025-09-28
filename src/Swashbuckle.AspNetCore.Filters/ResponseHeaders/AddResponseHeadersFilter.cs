using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.GetControllerAndActionAttributes<SwaggerResponseHeaderAttribute>();

            foreach (var attr in actionAttributes)
            {
                foreach (var statusCode in attr.StatusCodes)
                {
                    var response = operation.Responses.FirstOrDefault(x => x.Key == (statusCode).ToString(CultureInfo.InvariantCulture)).Value;

                    if (response != null)
                    {
                        //if (response.Headers == null)
                        //{
                        //    response.Headers = new Dictionary<string, IOpenApiHeader>();
                        //}

                        //response.Headers.Add(attr.Name, new OpenApiHeader { Description = attr.Description, Schema = new OpenApiSchema { Description = attr.Description, Type = attr.Type, Format = attr.Format } });
                    }
                }
            }
        }
    }
}
