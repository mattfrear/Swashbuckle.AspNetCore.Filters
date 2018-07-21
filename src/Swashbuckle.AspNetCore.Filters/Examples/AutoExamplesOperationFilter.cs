using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AutoExamplesOperationFilter : IOperationFilter
    {
        private readonly IServiceProvider serviceProvider;

        public AutoExamplesOperationFilter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetResponseExamples(context);

            foreach(var parameterDescription in context.ApiDescription.ParameterDescriptions)
            {
                var example = GetExampleFromServiceProvider(parameterDescription.Type);

                // todo, set the example on the schema
            }
        }

        private void SetResponseExamples(OperationFilterContext context)
        {
            var responseAttributes = context.GetControllerAndActionAttributes<SwaggerResponseAttribute>();

            foreach (var response in responseAttributes)
            {
                var example = GetExampleFromServiceProvider(response.Type);

                // todo, set the example on the operation
            }
        }

        private object GetExampleFromServiceProvider(Type type)
        {
            if (type == null || type == typeof(void) || !type.GetTypeInfo().IsClass)
            {
                return null;
            }

            var exampleProviderType = typeof(IAutoExamplesProvider<>).MakeGenericType(type);
            object exampleProviderObject = serviceProvider.GetService(exampleProviderType);

            if (exampleProviderObject == null)
            {
                return null;
            }

            var methodInfo = exampleProviderType.GetMethod("GetExamples");
            var example = methodInfo.Invoke(exampleProviderObject, null); // yay, we've got the example! Now just need to set it.
            return example;
        }
    }
}