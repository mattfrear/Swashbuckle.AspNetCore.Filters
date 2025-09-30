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
                        // In Microsoft.OpenApi 2.0 response.Headers is null and readonly, so we need to create a new OpenApiResponse and replace it in the Responses dictionary

                        var existingHeaders = response.Headers ?? new Dictionary<string, IOpenApiHeader>();
                        var newHeaders = new Dictionary<string, IOpenApiHeader>(existingHeaders)
                        {
                            [attr.Name] = new OpenApiHeader
                            {
                                Description = attr.Description,
                                Schema = new OpenApiSchema { Description = attr.Description, Type = attr.Type, Format = attr.Format }
                            }
                        };

                        var newResponse = new OpenApiResponse
                        {
                            Description = response.Description,
                            Content = response.Content,
                            Headers = newHeaders,
                            Links = response.Links,
                            Extensions = response.Extensions
                        };

                        operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)] = newResponse;
                    }
                }
            }
        }
    }
}
