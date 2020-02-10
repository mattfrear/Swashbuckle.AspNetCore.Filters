using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;

        public ResponseExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
        }

        public void SetResponseExampleForStatusCode(
            OutputFormatterSelector outputFormatterSelector,
            OpenApiOperation operation,
            int statusCode,
            object example,
            IContractResolver contractResolver = null,
            JsonConverter jsonConverter = null)
        {
            if (example == null)
            {
                return;
            }

            var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode.ToString());

            if (response.Equals(default(KeyValuePair<string, OpenApiResponse>)) == false && response.Value != null)
            {
                var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);
                var jsonExample = new OpenApiRawString(jsonFormatter.FormatJson(example, serializerSettings));

                OpenApiString xmlExample = null;
                if (response.Value.Content.Keys.Any(k => k.Contains("xml")))
                {
                    xmlExample = new OpenApiString(example.XmlSerialize(outputFormatterSelector));
                }

                foreach (var content in response.Value.Content)
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
}
