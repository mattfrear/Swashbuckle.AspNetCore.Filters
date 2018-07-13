using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class ExamplesOperationFilter : IOperationFilter
    {
        private readonly IRequestExample requestExample;
        private readonly IResponseExample responseExample;

        public ExamplesOperationFilter(
            IRequestExample requestExample,
            IResponseExample responseExample)
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