using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Examples
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var responseAttributes = context.MethodInfo.GetCustomAttributes<SwaggerResponseHeaderAttribute>();

            foreach (var attr in responseAttributes)
            {
                var response = operation.Responses.FirstOrDefault(x => x.Key == ((int)attr.StatusCode).ToString(CultureInfo.InvariantCulture)).Value;

                if (response != null)
                {
                    if (response.Headers == null)
                    {
                        response.Headers = new Dictionary<string, Header>();
                    }

                    response.Headers.Add(attr.Name, new Header { Description = attr.Description, Type = attr.Type });
                }
            }
        }
    }
}
