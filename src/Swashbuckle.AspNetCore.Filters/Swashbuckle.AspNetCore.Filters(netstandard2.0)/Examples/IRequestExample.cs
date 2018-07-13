using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    public interface IRequestExample
    {
        void SetRequestModelExamples(Operation operation, ISchemaRegistry schemaRegistry, OperationFilterContext context);
    }
}
