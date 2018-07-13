using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    internal interface IResponseExample
    {
        void SetResponseModelExamples(Operation operation, OperationFilterContext context);
    }
}