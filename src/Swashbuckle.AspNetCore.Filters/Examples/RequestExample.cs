using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters.Extensions;
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

        public void SetRequestExampleForOperation(
            OpenApiOperation operation,
            object example,
            IContractResolver contractResolver = null,
            JsonConverter jsonConverter = null)
        {
            if (example == null)
            {
                return;
            }

            var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);

            var jsonExample = jsonFormatter.FormatJson(example, serializerSettings, includeMediaType: false);

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key.Contains("xml"))
                {
                    var xml = example.XmlSerialize();
                    content.Value.Example = new OpenApiString(xml);
                }
                else
                {
                    content.Value.Example = new OpenApiString(jsonExample);
                }
            }
        }

        private static string SchemaDefinitionName(Type requestType, OpenApiSchema schema)
        {
            string name = null;
            // var name = attr.RequestType.Name; // this doesn't work for generic types, so need to to schema.ref split
            var parts = schema.Reference.ReferenceV2.Split('/');

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
