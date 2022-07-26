using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void ExampleFilters(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
        }

        public static void ExampleFilters_PrioritizingExplicitlyDefinedExamples(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();
        }
    }
}
