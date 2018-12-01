using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class RequestExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;

        public RequestExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
        }

        public void SetRequestExampleForType(
            Operation operation,
            ISchemaRegistry schemaRegistry,
            Type requestType,
            object example,
            IContractResolver contractResolver = null,
            JsonConverter jsonConverter = null)
        {
            if (example == null)
            {
                return;
            }

            var schema = schemaRegistry.GetOrRegister(requestType);

            var bodyParameters = operation.Parameters.Where(p => p.In == "body").Cast<BodyParameter>();
            var bodyParameter = bodyParameters.FirstOrDefault(p => p?.Schema.Ref == schema.Ref || p.Schema?.Items?.Ref == schema.Ref);

            if (bodyParameter == null)
            {
                return; // The type in their [SwaggerRequestExample(typeof(requestType), ...] is not passed to their controller action method
            }

            var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);

            var formattedExample = jsonFormatter.FormatJson(example, serializerSettings, includeMediaType: false);

            string name = SchemaDefinitionName(requestType, schema);

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            // set the example on the object in the schema registry (this is what swagger-ui will display)
            if (schemaRegistry.Definitions.ContainsKey(name))
            {
                var definitionToUpdate = schemaRegistry.Definitions[name];
                if (definitionToUpdate.Example == null)
                {
                    definitionToUpdate.Example = formattedExample;
                }
            }
            else
            {
                bodyParameter.Schema.Example = formattedExample; // set example on the request paths/parameters/schema/example property
            }
        }

        private static string SchemaDefinitionName(Type requestType, Schema schema)
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
                if (requestType.GetTypeInfo().IsGenericType)
                {
                    // remove `# from the generic type name
                    var friendlyName = requestType.Name.Remove(requestType.Name.IndexOf('`'));
                    // for generic, Schema will be TypeName[GenericTypeName]
                    name = $"{friendlyName}[{string.Join(",", requestType.GetGenericArguments().Select(a => a.Name).ToList())}]";
                }
                else
                {
                    name = requestType.Name;
                }
            }

            return name;
        }
    }
}
