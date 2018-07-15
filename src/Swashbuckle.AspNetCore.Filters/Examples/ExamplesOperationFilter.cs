using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        public ExamplesOperationFilter(
            RequestExample requestExample,
            ResponseExample responseExample)
        {
            this.requestExample = requestExample;
            this.responseExample = responseExample;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            requestExample.SetRequestModelExamples(operation, context.SchemaRegistry, context);
            responseExample.SetResponseModelExamples(operation, context);
        }
    }
}