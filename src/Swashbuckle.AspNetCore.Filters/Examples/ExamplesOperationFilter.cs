using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

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

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestModelExamples(operation, context);
            SetResponseModelExamples(operation, context);
        }

        private void SetRequestModelExamples(Operation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in actionAttributes)
            {
                var examplesProvider = (IExamplesProvider)(serviceProvider.GetService(attr.ExamplesProviderType)
                    ?? Activator.CreateInstance(attr.ExamplesProviderType));

                object example = examplesProvider?.GetExamples();

                requestExample.SetRequestExampleForType(operation, context.SchemaRegistry, attr.RequestType, example, attr.ContractResolver, attr.JsonConverter);
            }
        }

        private void SetResponseModelExamples(Operation operation, OperationFilterContext context)
        {
            var responseAttributes = context.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();

            foreach (var attr in responseAttributes)
            {
                var examplesProvider = (IExamplesProvider)(serviceProvider.GetService(attr.ExamplesProviderType)
                    ?? Activator.CreateInstance(attr.ExamplesProviderType));

                object example = examplesProvider?.GetExamples();

                responseExample.SetResponseExampleForStatusCode(operation, attr.StatusCode, example, attr.ContractResolver, attr.JsonConverter);
            }
        }
    }
}