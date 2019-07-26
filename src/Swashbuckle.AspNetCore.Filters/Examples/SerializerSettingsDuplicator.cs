using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class SerializerSettingsDuplicator
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly SchemaGeneratorOptions schemaGeneratorOptions;

#if NETCOREAPP3_0
        public SerializerSettingsDuplicator(IOptions<MvcNewtonsoftJsonOptions> mvcJsonOptions, IOptions<SchemaGeneratorOptions> schemaGeneratorOptions)
        {
            this.jsonSerializerSettings = mvcJsonOptions.Value.SerializerSettings;
            this.schemaGeneratorOptions = schemaGeneratorOptions.Value;
        }
#else
        public SerializerSettingsDuplicator(IOptions<MvcJsonOptions> mvcJsonOptions, IOptions<SchemaGeneratorOptions> schemaGeneratorOptions)
        {
            this.jsonSerializerSettings = mvcJsonOptions.Value.SerializerSettings;
            this.schemaGeneratorOptions = schemaGeneratorOptions.Value;
        }
#endif
        public JsonSerializerSettings SerializerSettings(IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter)
        {
            var serializerSettings = DuplicateSerializerSettings(jsonSerializerSettings);
            if (attributeContractResolver != null)
            {
                serializerSettings.ContractResolver = attributeContractResolver;
            }
            else if (schemaGeneratorOptions.IgnoreObsoleteProperties)
            {
                serializerSettings.ContractResolver = new ExcludeObsoletePropertiesResolver();
            }

            if (attributeJsonConverter != null)
            {
                serializerSettings.Converters.Add(attributeJsonConverter);
            }

            return serializerSettings;
        }

        // Duplicate the controller's serializer settings because I don't want to overwrite them
        private static JsonSerializerSettings DuplicateSerializerSettings(JsonSerializerSettings controllerSerializerSettings)
        {
            if (controllerSerializerSettings == null)
            {
                return new JsonSerializerSettings();
            }

            return new JsonSerializerSettings
            {
#if NETSTANDARD1_6
                Binder = controllerSerializerSettings.Binder,
#else
                SerializationBinder = controllerSerializerSettings.SerializationBinder,
#endif
                Converters = new List<JsonConverter>(controllerSerializerSettings.Converters),
                CheckAdditionalContent = controllerSerializerSettings.CheckAdditionalContent,
                ConstructorHandling = controllerSerializerSettings.ConstructorHandling,
                Context = controllerSerializerSettings.Context,
                ContractResolver = controllerSerializerSettings.ContractResolver,
                Culture = controllerSerializerSettings.Culture,
                DateFormatHandling = controllerSerializerSettings.DateFormatHandling,
                DateFormatString = controllerSerializerSettings.DateFormatString,
                DateParseHandling = controllerSerializerSettings.DateParseHandling,
                DateTimeZoneHandling = controllerSerializerSettings.DateTimeZoneHandling,
                DefaultValueHandling = controllerSerializerSettings.DefaultValueHandling,
                Error = controllerSerializerSettings.Error,
                Formatting = controllerSerializerSettings.Formatting,
                MaxDepth = controllerSerializerSettings.MaxDepth,
                MissingMemberHandling = controllerSerializerSettings.MissingMemberHandling,
                NullValueHandling = controllerSerializerSettings.NullValueHandling,
                ObjectCreationHandling = controllerSerializerSettings.ObjectCreationHandling,
                PreserveReferencesHandling = controllerSerializerSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = controllerSerializerSettings.ReferenceLoopHandling,
                TypeNameHandling = controllerSerializerSettings.TypeNameHandling
            };
        }
    }
}
