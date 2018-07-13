using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class SerializerSettingsDuplicator
    {
        private readonly IOptions<MvcJsonOptions> mvcJsonOptions;

        public SerializerSettingsDuplicator(IOptions<MvcJsonOptions> mvcJsonOptions)
        {
            this.mvcJsonOptions = mvcJsonOptions;
        }

        public JsonSerializerSettings SerializerSettings(IContractResolver attributeContractResolver, JsonConverter attributeJsonConverter)
        {
            var serializerSettings = DuplicateSerializerSettings(mvcJsonOptions.Value.SerializerSettings);
            if (attributeContractResolver != null)
            {
                serializerSettings.ContractResolver = attributeContractResolver;
            }
            serializerSettings.NullValueHandling = NullValueHandling.Ignore; // ignore nulls on any RequestExample properies because swagger does not support null objects https://github.com/OAI/OpenAPI-Specification/issues/229

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
