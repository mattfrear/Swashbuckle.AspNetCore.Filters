using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ResponseExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            MvcOutputFormatter mvcOutputFormatter)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.mvcOutputFormatter = mvcOutputFormatter;
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

            var examplesConverter = new ExamplesConverter(jsonFormatter, mvcOutputFormatter, serializerSettings);

            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                SetSingleResponseExampleForStatusCode(response, example, examplesConverter);
            }
            else
            {
                SetMultipleResponseExampleForStatusCode(response, multiple, examplesConverter);
            }
        }
        
        private void SetSingleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            object example,
            ExamplesConverter examplesConverter)
        {
            var jsonExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleXml(example));

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

        private void SetMultipleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
            );

            var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
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
