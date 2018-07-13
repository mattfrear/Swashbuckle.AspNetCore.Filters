using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class ResponseExample : IResponseExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly IServiceProvider serviceProvider;

        public ResponseExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            IServiceProvider serviceProvider = null)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.serviceProvider = serviceProvider;
        }

        public void SetResponseModelExamples(Operation operation, OperationFilterContext context)
        {
            var responseAttributes = context.MethodInfo.GetCustomAttributes<SwaggerResponseExampleAttribute>().ToList();

            responseAttributes.AddRange(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes<SwaggerResponseExampleAttribute>());

            foreach (var attr in responseAttributes)
            {
                var statusCode = attr.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false && response.Value != null)
                {
                    var examplesProvider = ExamplesProvider(serviceProvider, attr.ExamplesProviderType);

                    var serializerSettings = serializerSettingsDuplicator.SerializerSettings(attr.ContractResolver, attr.JsonConverter);

                    response.Value.Examples = jsonFormatter.FormatJson(examplesProvider, serializerSettings, true);
                }
            }
        }

        private static IExamplesProvider ExamplesProvider(IServiceProvider services, Type examplesProviderType)
        {
            var provider = services == null
                ? (IExamplesProvider)Activator.CreateInstance(examplesProviderType)
                : (IExamplesProvider)services.GetService(examplesProviderType)
                  ?? (IExamplesProvider)Activator.CreateInstance(examplesProviderType);
            return provider;
        }
    }
}
