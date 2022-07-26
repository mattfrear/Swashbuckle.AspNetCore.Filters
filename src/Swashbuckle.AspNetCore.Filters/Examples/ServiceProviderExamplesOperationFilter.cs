using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
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

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SetRequestExamples(operation, context);
            SetResponseExamples(operation, context);
        }

        private void SetRequestExamples(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.GetControllerAndActionAttributes<SwaggerRequestExampleAttribute>();

            foreach (var parameterDescription in context.ApiDescription.ParameterDescriptions)
            {
                if (actionAttributes.Any(a => a.RequestType == parameterDescription.Type))
                {
                    continue; // if [SwaggerRequestExample] is defined, then let ExamplesOperationFilter define the example
                }

                var example = serviceProvider.GetExampleForType(parameterDescription.Type);
                if (parameterDescription.Source == BindingSource.Body)
                {
                    requestExample.SetRequestBodyExampleForOperation(operation, context.SchemaRepository, parameterDescription.Type, example);
                }
            }
        }

        private class StatusCodeWithType
        {
            public StatusCodeWithType(int statusCode, Type type)
            {
                StatusCode = statusCode;
                Type = type;
            }

            public int StatusCode { get; }
            public Type Type { get; }
        }

        private void SetResponseExamples(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>().Select(a => new StatusCodeWithType(a.StatusCode, a.ExamplesProviderType));
            var responseAttributes = context.GetControllerAndActionAttributes<ProducesResponseTypeAttribute>().Select(a => new StatusCodeWithType(a.StatusCode, a.Type));
            var autodetectedResponses = context.ApiDescription.SupportedResponseTypes.Select(r => new StatusCodeWithType(r.StatusCode, r.Type));

            var responses = responseAttributes.Concat(autodetectedResponses);

            foreach (var response in responses)
            {
                if (actionAttributes.Any(a => a.StatusCode == response.StatusCode))
                {
                    continue; // if [SwaggerResponseExample] is defined, then let ExamplesOperationFilter define the example
                }

                var example = serviceProvider.GetExampleForType(response.Type);

                responseExample.SetResponseExampleForStatusCode(operation, response.StatusCode, example);
            }
        }
    }
}