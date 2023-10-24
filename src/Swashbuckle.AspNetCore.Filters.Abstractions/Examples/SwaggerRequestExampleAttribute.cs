using System;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Adds example requests to your controller endpoints.
    /// See: https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters
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
        public SwaggerRequestExampleAttribute(Type requestType, Type examplesProviderType)
        {
            RequestType = requestType;
            ExamplesProviderType = examplesProviderType;

            // todo - can (and should) I introduce a check here to check that the examplesProviderType's ImplementedInterfaces contains IExamplesProvider of requestType
            // Remember that there was an issue with ListPeopleRequestExample which is now commented out.
        }

        public Type ExamplesProviderType { get; }

        public Type RequestType { get; }
    }
}