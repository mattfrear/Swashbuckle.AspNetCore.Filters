using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ServiceProviderExamplesOperationFilter : IOperationFilter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly RequestExample requestExample;
        private readonly ResponseExample responseExample;

        public ServiceProviderExamplesOperationFilter(
            IServiceProvider serviceProvider,
            RequestExample requestExample,
            ResponseExample responseExample)
        {
            this.serviceProvider = serviceProvider;
            this.requestExample = requestExample;
            this.responseExample = responseExample;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestExamples(operation, context);
            SetResponseExamples(context);
        }

        private void SetRequestExamples(Operation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerRequestExampleAttribute>();
            if (actionAttributes.Any())
            {
                return; // if [SwaggerRequestExample] is defined, then let ExamplesOperationFilter define the example
            }

            foreach (var parameterDescription in context.ApiDescription.ParameterDescriptions)
            {
                var example = GetExampleForTypeFromServiceProvider(parameterDescription.Type);

                requestExample.SetRequestExampleForType(operation, context.SchemaRegistry, parameterDescription.Type, example);
            }
        }

        private void SetResponseExamples(OperationFilterContext context)
        {
            var responseAttributes = context.GetControllerAndActionAttributes<SwaggerResponseAttribute>();

            foreach (var response in responseAttributes)
            {
                var example = GetExampleForTypeFromServiceProvider(response.Type);

                // todo, set the example on the operation
            }
        }

        private object GetExampleForTypeFromServiceProvider(Type type)
        {
            if (type == null || type == typeof(void) || !type.GetTypeInfo().IsClass)
            {
                return null;
            }

            var exampleProviderType = typeof(IExamplesProvider<>).MakeGenericType(type);
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