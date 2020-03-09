using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatterProvider
    {
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly IOptions<MvcOptions> mvcOptions;

        public JsonFormatterProvider(SerializerSettingsDuplicator serializerSettingsDuplicator, IOptions<MvcOptions> mvcOptions)
        {
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.mvcOptions = mvcOptions;
        }

        public IJsonFormatter GetFormatter(IContractResolver contractResolver = null, JsonConverter jsonConverter = null)
        {
            throw new NotImplementedException();
        }
    }
}