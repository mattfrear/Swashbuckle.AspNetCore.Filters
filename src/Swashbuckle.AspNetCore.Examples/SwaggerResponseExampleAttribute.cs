using System;
using System.Net;

namespace Swashbuckle.AspNetCore.Examples
{
    /// <summary>
    /// This is used for generating Swagger documentation. Should be used in conjuction with SwaggerResponse - will add examples to SwaggerResponse.
    /// See https://mattfrear.com/2015/04/21/generating-swagger-example-responses-with-swashbuckle/
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseExampleAttribute : Attribute
    {
        public SwaggerResponseExampleAttribute(HttpStatusCode statusCode, Type examplesProviderType)
        {
            StatusCode = statusCode;
            ExamplesProviderType = examplesProviderType;
        }

        public Type ExamplesProviderType { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
