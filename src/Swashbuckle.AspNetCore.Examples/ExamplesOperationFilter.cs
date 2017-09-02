using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Examples
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        private static IServiceProvider _services;

        public ExamplesOperationFilter(IServiceProvider services = null)
        {
            _services = services;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestModelExamples(operation, context.SchemaRegistry, context.ApiDescription);
            SetResponseModelExamples(operation, context.ApiDescription);
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, apiDescription);
        }

        private static void SetRequestModelExamples(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription.ActionAttributes();
            var swaggerRequestAttributes = actionAttributes.Where(r => r.GetType() == typeof(SwaggerRequestExampleAttribute));

            foreach (var attribute in swaggerRequestAttributes)
            {
                var attr = (SwaggerRequestExampleAttribute)attribute;
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var request = operation.Parameters.FirstOrDefault(p => p.In == "body"/* && p.schema.@ref == schema.@ref */);

                if (request != null)
                {
                    var provider = _services == null
                        ? (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType)
                        : (IExamplesProvider)_services.GetService(attr.ExamplesProviderType)
                        ?? (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var parts = schema.Ref?.Split('/');
                    if (parts == null)
                    {
                        continue;
                    }

                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        var serializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = attr.ContractResolver,
                            NullValueHandling = NullValueHandling.Ignore // ignore null values because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229
                        };

                        if (attr.JsonConverter != null)
                        {
                            serializerSettings.Converters.Add(attr.JsonConverter);
                        }

                        definitionToUpdate.Example = ((dynamic)FormatAsJson(provider, serializerSettings))["application/json"];
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription.ActionAttributes();
            var swaggerResponseExampleAttributes = actionAttributes.Where(r => r.GetType() == typeof(SwaggerResponseExampleAttribute));

            foreach (var attribute in swaggerResponseExampleAttributes)
            {
                var attr = (SwaggerResponseExampleAttribute)attribute;
                var statusCode = attr.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var provider = _services == null
                            ? (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType)
                            : (IExamplesProvider)_services.GetService(attr.ExamplesProviderType)
                              ?? (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                        var serializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = attr.ContractResolver,
                            NullValueHandling = NullValueHandling.Ignore
                        };

                        if (attr.JsonConverter != null)
                        {
                            serializerSettings.Converters.Add(attr.JsonConverter);
                        }

                        response.Value.Examples = ConvertToDesiredCase(provider.GetExamples(), serializerSettings);
                    }
                }
            }
        }

        private static object ConvertToDesiredCase(object examples, JsonSerializerSettings serializerSettings)
        {
            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            return JsonConvert.DeserializeObject(jsonString);
        }

        private static object FormatAsJson(IExamplesProvider provider, JsonSerializerSettings serializerSettings)
        {
            var examples = new Dictionary<string, object>
            {
                {
                    "application/json", provider.GetExamples()
                }
            };

            return ConvertToDesiredCase(examples, serializerSettings);
        }
    }
}