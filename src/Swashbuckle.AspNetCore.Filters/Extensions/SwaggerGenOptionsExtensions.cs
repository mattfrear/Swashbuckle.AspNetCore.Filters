using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void ExampleFilters(this SwaggerGenOptions swaggerGenOptions)
        {
            //Suggested fix 2
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();

        }
    }
}
