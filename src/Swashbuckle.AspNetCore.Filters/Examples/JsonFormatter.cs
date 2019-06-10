using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatter
    {
        public string FormatJson(object examples, JsonSerializerSettings serializerSettings)
        {
            serializerSettings.Formatting = Formatting.Indented;
            return JsonConvert.SerializeObject(examples, serializerSettings);
        }
    }
}
