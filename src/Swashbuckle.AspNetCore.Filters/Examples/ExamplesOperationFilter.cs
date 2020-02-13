using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <summary>
    /// Adds custom Request and Response examples.
    /// You should install it using the .AddSwaggerExamples() extension method
    /// </summary>
    internal class ExamplesOperationFilter : IOperationFilter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly RequestExample requestExample;
        private readonly ResponseExample responseExample;

        public ExamplesOperationFilter(
            IServiceProvider serviceProvider,
            RequestExample requestExample,
            ResponseExample responseExample)
        {
            this.serviceProvider = serviceProvider;
            this.requestExample = requestExample;
            this.responseExample = responseExample;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SetRequestModelExamples(operation, context);
            SetResponseModelExamples(operation, context);
        }

        private void SetRequestModelExamples(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in actionAttributes)
            {
                var example = serviceProvider.GetExampleWithExamplesProviderType(attr.ExamplesProviderType);

                requestExample.SetRequestExampleForOperation(
                    operation,
                    example,
                    attr.ContractResolver,
                    attr.JsonConverter);
            }
        }

        private void SetResponseModelExamples(OpenApiOperation operation, OperationFilterContext context)
        {
            var responseAttributes = context.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();

            foreach (var attr in responseAttributes)
            {
                var example = serviceProvider.GetExampleWithExamplesProviderType(attr.ExamplesProviderType);

                responseExample.SetResponseExampleForStatusCode(
                    operation,
                    attr.StatusCode,
                    example,
                    attr.ContractResolver,
                    attr.JsonConverter);
            }
        }
    }
}