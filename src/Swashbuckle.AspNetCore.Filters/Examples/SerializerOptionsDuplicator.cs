using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class SerializerOptionsDuplicator
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly SchemaGeneratorOptions schemaGeneratorOptions;


#if NETCOREAPP3_0
        public SerializerOptionsDuplicator(IOptions<JsonOptions> jsonOptions, IOptions<SchemaGeneratorOptions> schemaGeneratorOptions)
        {
            this.jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
            this.schemaGeneratorOptions = schemaGeneratorOptions.Value;
        }
#else
        public SerializerOptionsDuplicator(IOptions<SchemaGeneratorOptions> schemaGeneratorOptions)
        {
            this.jsonSerializerOptions = new JsonSerializerOptions();
            this.schemaGeneratorOptions = schemaGeneratorOptions.Value;
        }
#endif

        public JsonSerializerOptions SerializerOptions()
        {
            var serializerSettings = DuplicateSerializerSettings(jsonSerializerOptions);

            if (schemaGeneratorOptions.IgnoreObsoleteProperties)
            {
                // TODO THIS
            }

            return serializerSettings;
        }

        private static JsonSerializerOptions DuplicateSerializerSettings(JsonSerializerOptions controllerSerializerOptions)
        {
            if (controllerSerializerOptions == null)
            {
                return new JsonSerializerOptions();
            }

            var options = new JsonSerializerOptions
            {
                Encoder = controllerSerializerOptions.Encoder,
                MaxDepth = controllerSerializerOptions.MaxDepth,
                WriteIndented = controllerSerializerOptions.WriteIndented,
                AllowTrailingCommas = controllerSerializerOptions.AllowTrailingCommas,
                DefaultBufferSize = controllerSerializerOptions.DefaultBufferSize,
                DictionaryKeyPolicy = controllerSerializerOptions.DictionaryKeyPolicy,
                IgnoreNullValues = controllerSerializerOptions.IgnoreNullValues,
                PropertyNamingPolicy = controllerSerializerOptions.PropertyNamingPolicy,
                ReadCommentHandling = controllerSerializerOptions.ReadCommentHandling,
                IgnoreReadOnlyProperties = controllerSerializerOptions.IgnoreReadOnlyProperties,
                PropertyNameCaseInsensitive = controllerSerializerOptions.PropertyNameCaseInsensitive
            };

            foreach (var converter in controllerSerializerOptions.Converters)
            {
                options.Converters.Add(converter);
            }

            return options;
        }
    }
}