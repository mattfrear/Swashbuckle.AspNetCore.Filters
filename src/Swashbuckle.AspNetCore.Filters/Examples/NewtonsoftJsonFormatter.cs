using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class NewtonsoftJsonFormatter : IJsonFormatter
    {
        private readonly JsonSerializerSettings serializerSettings;

        public NewtonsoftJsonFormatter(JsonSerializerSettings serializerSettings)
        {
            serializerSettings.Formatting = Formatting.Indented;
            this.serializerSettings = serializerSettings;
        }

        public string FormatJson(object examples)
        {
            return JsonConvert.SerializeObject(examples, serializerSettings);
        }
    }
}