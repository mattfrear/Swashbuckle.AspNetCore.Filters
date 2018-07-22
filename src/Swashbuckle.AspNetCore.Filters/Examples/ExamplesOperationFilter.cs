using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <summary>
    /// Adds custom Request and Response examples.
    /// You should install it using the .AddSwaggerExamples() extension method
    /// </summary>
    internal class ExamplesOperationFilter : IOperationFilter
    {
        private readonly RequestExample requestExample;
        private readonly ResponseExample responseExample;
        private readonly ExamplesProviderFactory examplesProviderFactory;

        public ExamplesOperationFilter(
            RequestExample requestExample,
            ResponseExample responseExample,
            ExamplesProviderFactory examplesProviderFactory)
        {
            this.requestExample = requestExample;
            this.responseExample = responseExample;
            this.examplesProviderFactory = examplesProviderFactory;
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
                var examplesProvider = examplesProviderFactory.Create(attr.ExamplesProviderType);
                object example = examplesProvider.GetExamples();

                requestExample.SetRequestExampleForType(operation, context.SchemaRegistry, attr.RequestType, example, attr.ContractResolver, attr.JsonConverter);
            }
        }

        private void SetResponseModelExamples(Operation operation, OperationFilterContext context)
        {
            var responseAttributes = context.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();

            foreach (var attr in responseAttributes)
            {
                var examplesProvider = examplesProviderFactory.Create(attr.ExamplesProviderType);
                object example = examplesProvider.GetExamples();

                responseExample.SetResponseExampleForStatusCode(operation, attr.StatusCode, example, attr.ContractResolver, attr.JsonConverter);
            }
        }
    }
}