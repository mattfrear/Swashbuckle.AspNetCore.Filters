using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                var jsonExample = new Lazy<IOpenApiAny>(() => new OpenApiRawString(jsonFormatter.FormatJson(example, serializerSettings)));
                var xmlExample = new Lazy<IOpenApiAny>(() => new OpenApiString(example.XmlSerialize()));

                foreach (var content in operation.RequestBody.Content)
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

                foreach (var content in operation.RequestBody.Content)
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
