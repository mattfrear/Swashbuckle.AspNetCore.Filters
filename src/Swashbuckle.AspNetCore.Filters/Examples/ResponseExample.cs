using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly ExamplesProviderFactory examplesProviderFactory;

        public ResponseExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            ExamplesProviderFactory examplesProviderFactory)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.examplesProviderFactory = examplesProviderFactory;
        }

        public void SetResponseExampleForStatusCode(
            Operation operation,
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

            if (response.Equals(default(KeyValuePair<string, Response>)) == false && response.Value != null)
            {
                var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);
                response.Value.Examples = jsonFormatter.FormatJson(example, serializerSettings, includeMediaType: true);
            }

        }
    }
}
