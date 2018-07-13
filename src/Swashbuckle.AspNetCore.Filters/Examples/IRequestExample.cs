using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public interface IRequestExample
    {
        void SetRequestModelExamples(Operation operation, ISchemaRegistry schemaRegistry, OperationFilterContext context);
    }
}
