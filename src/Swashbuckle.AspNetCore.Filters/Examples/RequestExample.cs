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

            if (operation.RequestBody == null || operation.RequestBody.Content == null)
            {
                return;
            }

            var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);
            var jsonExample = new OpenApiString(jsonFormatter.FormatJson(example, serializerSettings));

            OpenApiString xmlExample = null;
            if (operation.RequestBody.Content.Keys.Any(k => k.Contains("xml")))
            {
                xmlExample = new OpenApiString(example.XmlSerialize());
            }

            foreach (var content in operation.RequestBody.Content)
            {
                content.Value.Example = content.Key.Contains("xml") ? xmlExample : jsonExample;
            }
        }
    }
}
