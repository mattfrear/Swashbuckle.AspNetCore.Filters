using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ExamplesOperationFilter(
            IServiceProvider serviceProvider,
            RequestExample requestExample,
            ResponseExample responseExample)
        {
            this.serviceProvider = serviceProvider;
            this.requestExample = requestExample;
            this.responseExample = responseExample;

            this.mvcOutputFormatter = new MvcOutputFormatter(serviceProvider.GetService<IOptions<MvcOptions>>(), serviceProvider.GetService<ILoggerFactory>());
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
                    mvcOutputFormatter,
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
                    mvcOutputFormatter,
                    operation,
                    attr.StatusCode,
                    example,
                    attr.ContractResolver,
                    attr.JsonConverter);
            }
        }
    }
}