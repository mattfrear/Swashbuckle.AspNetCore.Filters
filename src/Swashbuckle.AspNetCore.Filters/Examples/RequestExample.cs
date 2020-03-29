using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

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

            var examplesConverter = new ExamplesConverter(jsonFormatter, mvcOutputFormatter, serializerSettings);

            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                SetSingleRequestExampleForOperation(operation, example, examplesConverter);
            }
            else
            {
                SetMultipleRequestExamplesForOperation(operation, multiple, examplesConverter);
            }
        }

        private void SetSingleRequestExampleForOperation(
            OpenApiOperation operation,
            object example,
            ExamplesConverter examplesConverter)
        {
            var jsonExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleXml(example));

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

        private void SetMultipleRequestExamplesForOperation(
            OpenApiOperation operation,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
            );

            var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
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
