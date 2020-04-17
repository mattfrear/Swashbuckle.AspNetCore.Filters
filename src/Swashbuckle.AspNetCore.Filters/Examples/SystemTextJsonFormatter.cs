using System.Text.Json;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class SystemTextJsonFormatter : IJsonFormatter
    {
        private readonly JsonSerializerOptions serializerOptions;

        public SystemTextJsonFormatter(JsonSerializerOptions serializerOptions)
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