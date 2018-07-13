using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class RequestExample : IRequestExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly IServiceProvider serviceProvider;

        public RequestExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            IServiceProvider serviceProvider = null)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.serviceProvider = serviceProvider;
        }

        public void SetRequestModelExamples(Operation operation, ISchemaRegistry schemaRegistry, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in actionAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var bodyParameters = operation.Parameters.Where(p => p.In == "body").Cast<BodyParameter>();
                var request = bodyParameters.FirstOrDefault(p => p?.Schema.Ref == schema.Ref || p.Schema?.Items?.Ref == schema.Ref);

                if (request == null)
                {
                    continue; // The type in their [SwaggerRequestExample(typeof(requestType), ...] is not passed to their controller action method
                }

                var provider = ExamplesProvider(serviceProvider, attr.ExamplesProviderType);

                var serializerSettings = serializerSettingsDuplicator.SerializerSettings(attr.ContractResolver, attr.JsonConverter);

                var example = jsonFormatter.FormatJson(provider, serializerSettings, false);
                request.Schema.Example = example; // set example on the paths/parameters/schema/example property

                string name = SchemaDefinitionName(attr, schema);

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                // now that we have the name, additionally set the example on the object in the schema registry (this is what swagger-ui will display)
                if (schemaRegistry.Definitions.ContainsKey(name))
                {
                    var definitionToUpdate = schemaRegistry.Definitions[name];
                    if (definitionToUpdate.Example == null)
                    {
                        definitionToUpdate.Example = example;
                    }
                }
            }
        }

        private static string SchemaDefinitionName(SwaggerRequestExampleAttribute attr, Schema schema)
        {
            string name = null;
            // var name = attr.RequestType.Name; // this doesn't work for generic types, so need to to schema.ref split
            var parts = schema.Ref?.Split('/');

            if (parts != null)
            {
                name = parts.Last();
            }
            else
            {
                // schema.Ref can be null for some types, so we have to try get it by attr.RequestType.Name
                if (attr.RequestType.GetTypeInfo().IsGenericType)
                {
                    // remove `# from the generic type name
                    var friendlyName = attr.RequestType.Name.Remove(attr.RequestType.Name.IndexOf('`'));
                    // for generic, Schema will be TypeName[GenericTypeName]
                    name = $"{friendlyName}[{string.Join(",", attr.RequestType.GetGenericArguments().Select(a => a.Name).ToList())}]";
                }
                else
                {
                    name = attr.RequestType.Name;
                }
            }

            return name;
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
