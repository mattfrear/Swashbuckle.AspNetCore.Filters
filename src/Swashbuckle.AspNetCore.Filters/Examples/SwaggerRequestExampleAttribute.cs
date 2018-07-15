using Newtonsoft.Json.Serialization;
using System;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Adds example requests to your controller endpoints.
    /// See: https://github.com/mattfrear/Swashbuckle.AspNetCore.Examples
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerRequestExampleAttribute : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Add example data for a request
        /// </summary>
        /// <param name="requestType">The type passed to the request</param>
        /// <param name="examplesProviderType">A type that inherits from IExamplesProvider</param>
        /// <param name="contractResolver">An optional json contract Resolver if you want to override the one you use</param>
        /// <param name="jsonConverter">An optional jsonConverter to use, e.g. typeof(StringEnumConverter) will render strings as enums</param>
        public SwaggerRequestExampleAttribute(Type requestType, Type examplesProviderType, Type contractResolver = null, Type jsonConverter = null)
        {
            RequestType = requestType;
            ExamplesProviderType = examplesProviderType;
            JsonConverter = jsonConverter == null ? null : (JsonConverter)Activator.CreateInstance(jsonConverter);
            ContractResolver = contractResolver == null ? null : (IContractResolver)Activator.CreateInstance(contractResolver);
        }

        public Type ExamplesProviderType { get; }

        public JsonConverter JsonConverter { get; }

        public Type RequestType { get; }

        public IContractResolver ContractResolver { get; }
    }
}