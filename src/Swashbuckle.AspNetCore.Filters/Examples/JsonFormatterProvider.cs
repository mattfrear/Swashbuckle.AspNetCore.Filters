using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatterProvider
    {
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly SerializerOptionsDuplicator serializerOptionsDuplicator;
        private readonly SerializationMode defaultSerializationMode;

        public JsonFormatterProvider(SerializerSettingsDuplicator serializerSettingsDuplicator, SerializerOptionsDuplicator serializerOptionsDuplicator, IOptions<MvcOptions> mvcOptions)
        {
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.serializerOptionsDuplicator = serializerOptionsDuplicator;

#if NETCOREAPP3_0
            // NewtonsoftJsonFormatter will be present if consumer has called UseNewtonsoftJson in Startup.cs
            defaultSerializationMode = mvcOptions.Value.OutputFormatters.Any(x => x.GetType() == typeof(NewtonsoftJsonOutputFormatter))
                ? SerializationMode.Newtonsoft
                : SerializationMode.SystemTextJson;
#else
            defaultSerializationMode = SerializationMode.Newtonsoft;
#endif
        }

        public IJsonFormatter GetFormatter(IContractResolver contractResolver = null, JsonConverter jsonConverter = null)
        {
            if (UseNewtonsoft(contractResolver, jsonConverter))
                return new NewtonsoftJsonFormatter(serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter));

            return new SystemJsonFormatter(serializerOptionsDuplicator.SerializerOptions());
        }

        private bool UseNewtonsoft(IContractResolver contractResolver = null, JsonConverter jsonConverter = null) =>
            defaultSerializationMode == SerializationMode.Newtonsoft || contractResolver != null || jsonConverter != null;
    }

    public enum SerializationMode
    {
        Newtonsoft,
        SystemTextJson
    }
}