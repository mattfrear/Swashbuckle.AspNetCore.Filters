using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal interface IResponseExample
    {
        void SetResponseModelExamples(Operation operation, OperationFilterContext context);
    }
}