using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Enable Swashbuckle filters which provide sensible examples for requests and respnoses. Please implement IExamplesProvider<T>.
        /// </summary>
        /// <param name="swaggerGenOptions"></param>
        public static void ExampleFilters(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
        }

        /// <summary>
        /// Enable Swashbuckle filters which provide sensible examples for requests and respnoses. Please implement IExamplesProvider<T>.
        /// This option reverses the order the filters are registered which might be useful for some edge cases.
        /// See: https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters/pull/210#issuecomment-1195673783
        /// </summary>
        /// <param name="swaggerGenOptions"></param>
        public static void ExampleFilters_PrioritizingExplicitlyDefinedExamples(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();
        }
    }
}
