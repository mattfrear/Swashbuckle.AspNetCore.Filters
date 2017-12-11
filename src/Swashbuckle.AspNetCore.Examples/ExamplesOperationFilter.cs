using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Swashbuckle.AspNetCore.Examples
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        private static IServiceProvider _services;
        private static IOptions<MvcJsonOptions> _mvcJsonOptions;

        public ExamplesOperationFilter(IOptions<MvcJsonOptions> mvcJsonOptions, IServiceProvider services = null)
        {
            _services = services;
            _mvcJsonOptions = mvcJsonOptions;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetRequestModelExamples(operation, context.SchemaRegistry, context.ApiDescription);
            SetResponseModelExamples(operation, context.ApiDescription);
        }

        private static void SetRequestModelExamples(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription
                .ActionAttributes()
                .OfType<SwaggerRequestExampleAttribute>();

            foreach (var attr in actionAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var bodyParameters = operation.Parameters.Where(p => p.In == "body").Cast<BodyParameter>();
                var request = bodyParameters.FirstOrDefault(p => p.Schema.Ref == schema.Ref || p.Schema.Items.Ref == schema.Ref);

                if (request != null)
                {
                    var provider = ExamplesProvider(_services, attr.ExamplesProviderType);

                    // var name = attr.RequestType.Name; // this doesn't work for generic types, so need to to schema.ref split
                    var parts = schema.Ref?.Split('/');
                    if (parts == null)
                    {
                        continue;
                    }

                    var name = parts.Last();

                    if (schemaRegistry.Definitions.ContainsKey(name))
                    {
                        var definitionToUpdate = schemaRegistry.Definitions[name];
                        var serializerSettings = SerializerSettings(attr.ContractResolver, attr.JsonConverter);

                        definitionToUpdate.Example = FormatJson(provider, serializerSettings, false);
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription
                .ActionAttributes()
                .OfType<SwaggerResponseExampleAttribute>();

            foreach (var attr in actionAttributes)
            {
                var statusCode = attr.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var provider = ExamplesProvider(_services, attr.ExamplesProviderType);

                        var serializerSettings = SerializerSettings(attr.ContractResolver, attr.JsonConverter);

                        response.Value.Examples = FormatJson(provider, serializerSettings, true);
                    }
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

        private static object FormatJson(IExamplesProvider provider, JsonSerializerSettings serializerSettings, bool includeMediaType)
        {
            object examples;
            if (includeMediaType)
            {
                examples = new Dictionary<string, object>
                {
                    {
                        "application/json", provider.GetExamples()
                    }
                };
            }
            else
            {
                examples = provider.GetExamples();
            }

            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }

        private static JsonSerializerSettings SerializerSettings(IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter)
        {
            var serializerSettings = DuplicateSerializerSettings(_mvcJsonOptions.Value.SerializerSettings);
            if (attributeContractResolver != null)
            {
                serializerSettings.ContractResolver = attributeContractResolver;
            }
            serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any RequestExample properies because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229

            if (attributeJsonConverter != null)
            {
                serializerSettings.Converters.Add(attributeJsonConverter);
            }

            return serializerSettings;
        }

        // Duplicate the controller's serializer settings because I don't want to overwrite them
        private static JsonSerializerSettings DuplicateSerializerSettings(JsonSerializerSettings controllerSerializerSettings)
        {
            if (controllerSerializerSettings == null)
            {
                return new JsonSerializerSettings();
            }

            return new JsonSerializerSettings
            {
                Binder = controllerSerializerSettings.Binder,
                Converters = new List<JsonConverter>(controllerSerializerSettings.Converters),
                CheckAdditionalContent = controllerSerializerSettings.CheckAdditionalContent,
                ConstructorHandling = controllerSerializerSettings.ConstructorHandling,
                Context = controllerSerializerSettings.Context,
                ContractResolver = controllerSerializerSettings.ContractResolver,
                Culture = controllerSerializerSettings.Culture,
                DateFormatHandling = controllerSerializerSettings.DateFormatHandling,
                DateParseHandling = controllerSerializerSettings.DateParseHandling,
                DateTimeZoneHandling = controllerSerializerSettings.DateTimeZoneHandling,
                DefaultValueHandling = controllerSerializerSettings.DefaultValueHandling,
                Error = controllerSerializerSettings.Error,
                Formatting = controllerSerializerSettings.Formatting,
                MaxDepth = controllerSerializerSettings.MaxDepth,
                MissingMemberHandling = controllerSerializerSettings.MissingMemberHandling,
                NullValueHandling = controllerSerializerSettings.NullValueHandling,
                ObjectCreationHandling = controllerSerializerSettings.ObjectCreationHandling,
                PreserveReferencesHandling = controllerSerializerSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = controllerSerializerSettings.ReferenceLoopHandling,
                TypeNameHandling = controllerSerializerSettings.TypeNameHandling,
            };
        }
    }
}