using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ServiceProviderExamplesOperationFilter : IOperationFilter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly RequestExample requestExample;
        private readonly ResponseExample responseExample;

        public ServiceProviderExamplesOperationFilter(
            IServiceProvider serviceProvider,
            RequestExample requestExample,
            ResponseExample responseExample)
        {
            this.serviceProvider = serviceProvider;
            this.requestExample = requestExample;
            this.responseExample = responseExample;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestExamples(operation, context);
            SetResponseExamples(operation, context);
        }

        private void SetRequestExamples(Operation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerRequestExampleAttribute>();
            
            foreach (var parameterDescription in context.ApiDescription.ParameterDescriptions)
            {
                if (actionAttributes.Any(a => a.RequestType == parameterDescription.Type))
                {
                    continue; // if [SwaggerRequestExample] is defined, then let ExamplesOperationFilter define the example
                }

                var example = GetExampleForTypeFromServiceProvider(parameterDescription.Type);

                requestExample.SetRequestExampleForType(operation, context.SchemaRegistry, parameterDescription.Type, example);
            }
        }

        private class StatusCodeWithType
        {
            public StatusCodeWithType(int statusCode, Type type)
            {
                StatusCode = statusCode;
                Type = type;
            }

            public int StatusCode { get; }
            public Type Type { get; }
        }

        private void SetResponseExamples(Operation operation, OperationFilterContext context)
        {

            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerResponseExampleAttribute>();
            var responseAttributes = context.GetControllerAndActionAttributes<ProducesResponseTypeAttribute>().Select(a => new StatusCodeWithType(a.StatusCode, a.Type));
            var autodetectedResponses = context.ApiDescription.SupportedResponseTypes.Select(r => new StatusCodeWithType(r.StatusCode, r.Type));

            var responses = responseAttributes.Concat(autodetectedResponses);

            foreach (var response in responses)
            {
                if (actionAttributes.Any(a => a.StatusCode == response.StatusCode))
                {
                    continue; // if [SwaggerResponseExample] is defined, then let ExamplesOperationFilter define the example
                }

                var example = GetExampleForTypeFromServiceProvider(response.Type);

                responseExample.SetResponseExampleForStatusCode(operation, response.StatusCode, example);
            }
        }

        private object GetExampleForTypeFromServiceProvider(Type type)
        {
            if (type == null || type == typeof(void) || IsPrimitiveType())
            {
                return null;
            }

            bool IsPrimitiveType()
            {
                return !type.GetTypeInfo().IsClass
                    && !type.GetTypeInfo().IsGenericType
                    && !type.GetTypeInfo().IsInterface;
            }

            var exampleProviderType = typeof(IExamplesProvider<>).MakeGenericType(type);
            object exampleProviderObject = serviceProvider.GetService(exampleProviderType);

            if (exampleProviderObject == null)
            {
                return null;
            }

            var methodInfo = exampleProviderType.GetMethod("GetExamples");
            var example = methodInfo.Invoke(exampleProviderObject, null); // yay, we've got the example! Now just need to set it.
            return example;
        }
    }
}