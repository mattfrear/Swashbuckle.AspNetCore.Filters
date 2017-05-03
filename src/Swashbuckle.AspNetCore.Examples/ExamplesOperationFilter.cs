using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Examples
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestModelExamples(operation, context.SchemaRegistry, context.ApiDescription);
            SetResponseModelExamples(operation, context.SchemaRegistry, context.ApiDescription);
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, schemaRegistry, apiDescription);
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
                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var parts = schema.Ref.Split('/');
                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        definitionToUpdate.Example = ((dynamic)FormatAsJson(provider))["application/json"];
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription.ActionAttributes();
            var swaggerRequestAttributes = actionAttributes.Where(r => r.GetType() == typeof(SwaggerRequestExampleAttribute));

            foreach (var attribute in swaggerRequestAttributes)
            {
                var attr = (SwaggerResponseExampleAttribute)attribute;
                var statusCode = ((int)attr.StatusCode).ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);
                        response.Value.Examples = FormatAsJson(provider);
                    }
                }
            }
        }

        private static object ConvertToCamelCase(Dictionary<string, object> examples)
        {
            var jsonString = JsonConvert.SerializeObject(examples, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return JsonConvert.DeserializeObject(jsonString);
        }

        private static object FormatAsJson(IExamplesProvider provider)
        {
            var examples = new Dictionary<string, object>
            {
                {
                    "application/json", provider.GetExamples()
                }
            };

            return ConvertToCamelCase(examples);
        }
    }
}