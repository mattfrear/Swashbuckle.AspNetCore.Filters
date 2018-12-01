using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class SerializerSettingsDuplicator
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public SerializerSettingsDuplicator(IOptions<MvcJsonOptions> mvcJsonOptions)
        {
            this.jsonSerializerSettings = mvcJsonOptions.Value.SerializerSettings;
        }

        public JsonSerializerSettings SerializerSettings(IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter)
        {
            var serializerSettings = DuplicateSerializerSettings(jsonSerializerSettings);
            if (attributeContractResolver != null)
            {
                serializerSettings.ContractResolver = attributeContractResolver;
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
                // Binder = controllerSerializerSettings.Binder, // Obsolete in Json.NET 10.0 - experiment
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
                TypeNameHandling = controllerSerializerSettings.TypeNameHandling,
            };
        }
    }
}
