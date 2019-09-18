using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;

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
            if (response.Equals(default(KeyValuePair<string, OpenApiResponse>)) != false || response.Value == null)
            {
                return;
            }

            var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);
            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                var jsonExample = new Lazy<IOpenApiAny>(() => new OpenApiRawString(jsonFormatter.FormatJson(example, serializerSettings)));
                var xmlExample = new Lazy<IOpenApiAny>(() => new OpenApiString(example.XmlSerialize()));

                foreach (var content in response.Value.Content)
                {
                    if (content.Key.Contains("xml"))
                    {
                        content.Value.Example = xmlExample.Value;
                    }
                    else
                    {
                        content.Value.Example = jsonExample.Value;
                    }
                }
            }
            else
            {
                var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                    multiple.ToDictionary(
                        ex => ex.Name,
                        ex => new OpenApiExample {
                            Summary = ex.Summary,
                            Value = new OpenApiRawString(jsonFormatter.FormatJson(ex.Value, serializerSettings)),
                        }
                    )
                );

                var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                    multiple.ToDictionary(
                        ex => ex.Name,
                        ex => new OpenApiExample {
                            Summary = ex.Summary,
                            Value = new OpenApiString(ex.XmlSerialize()),
                        }
                    )
                );

                foreach (var content in response.Value.Content)
                {
                    if (content.Key.Contains("xml"))
                    {
                        content.Value.Examples = xmlExamples.Value;
                    }
                    else
                    {
                        content.Value.Examples = jsonExamples.Value;
                    }
                }
            }
        }
    }
}
