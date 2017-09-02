using Newtonsoft.Json.Serialization;
using System;

namespace Swashbuckle.AspNetCore.Examples
{
    /// <inheritdoc />
    /// <summary>
    /// Adds example requests to your controller endpoints.
    /// See https://mattfrear.com/2016/01/25/generating-swagger-example-requests-with-swashbuckle/
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerRequestExampleAttribute : Attribute
    {
        public SwaggerRequestExampleAttribute(Type requestType, Type examplesProviderType, Type contractResolver = null)
        {
            RequestType = requestType;
            ExamplesProviderType = examplesProviderType;
            ContractResolver = (IContractResolver)Activator.CreateInstance(contractResolver ?? typeof(CamelCasePropertyNamesContractResolver));
        }

        public Type ExamplesProviderType { get; private set; }

        public Type RequestType { get; private set; }

        public IContractResolver ContractResolver { get; private set; }
    }
}