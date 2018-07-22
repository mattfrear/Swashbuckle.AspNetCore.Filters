using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddSwaggerExamples(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>();
            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>();
        }
    }
}
