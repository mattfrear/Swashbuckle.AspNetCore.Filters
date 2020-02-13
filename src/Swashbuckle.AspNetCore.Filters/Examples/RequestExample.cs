using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class RequestExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public RequestExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            MvcOutputFormatter mvcOutputFormatter)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.mvcOutputFormatter = mvcOutputFormatter;
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
            var jsonExample = new OpenApiRawString(jsonFormatter.FormatJson(example, serializerSettings));

            OpenApiString xmlExample = null;
            if (operation.RequestBody.Content.Keys.Any(k => k.Contains("xml")))
            {
                xmlExample = new OpenApiString(example.XmlSerialize(mvcOutputFormatter));
            }

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key.Contains("xml"))
                {
                    content.Value.Example = xmlExample;
                }
                else
                {
                    content.Value.Example = jsonExample;
                }
            }
        }
    }
}
