using System.Text.Json;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class SystemJsonFormatter : IJsonFormatter
    {
        private readonly JsonSerializerOptions serializerOptions;

        public SystemJsonFormatter(JsonSerializerOptions serializerOptions)
        {
            serializerOptions.WriteIndented = true;
            this.serializerOptions = serializerOptions;
        }

        public string FormatJson(object examples)
        {
            return JsonSerializer.Serialize(examples, serializerOptions);
        }
    }
}