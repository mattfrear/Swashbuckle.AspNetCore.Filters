using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatter
    {
        public object FormatJson(object examples, JsonSerializerSettings serializerSettings, bool includeMediaType)
        {
            if (includeMediaType)
            {
                examples = new Dictionary<string, object>
                {
                    {
                        "application/json", examples
                    }
                };
            }

            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }
    }
}
